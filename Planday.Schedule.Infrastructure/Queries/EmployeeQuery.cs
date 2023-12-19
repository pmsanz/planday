using Dapper;
using Microsoft.Data.Sqlite;
using Planday.Schedule.Infrastructure.Providers.Interfaces;
using Planday.Schedule.Queries;

namespace Planday.Schedule.Infrastructure.Queries
{
    public class EmployeeQuery : IEmployeeQuery
    {
        private readonly IConnectionStringProvider _connectionStringProvider;

        private const string SqlGetEmployeeById = @"SELECT Id,Name FROM Employee WHERE Id = {0};";
        private const string SqlCheckOverlappingShifts = @"
        SELECT COUNT(*)
        FROM Shift
        WHERE EmployeeId = {2}
        AND (
            ('{0}' BETWEEN Start AND End)
            OR ('{1}' BETWEEN Start AND End)
            OR (Start BETWEEN '{0}' AND '{1}')
        );";
        private record EmployeeDto(long Id, string Name);

        public EmployeeQuery(IConnectionStringProvider connectionStringProvider)
        {
            _connectionStringProvider = connectionStringProvider;
        }

        public async Task<Employee?> GetEmployeeById(long id)
        {
            await using var sqlConnection = new SqliteConnection(_connectionStringProvider.GetConnectionString());
            string SqlGetEmployeeByIdFormated = string.Format(SqlGetEmployeeById, id);

            var employeeDtos = await sqlConnection.QueryAsync<EmployeeDto>(SqlGetEmployeeByIdFormated);

            var employees = employeeDtos.Select(x =>
                new Employee(x.Id, x.Name));

            return employees.FirstOrDefault();
        }

        public async Task<bool> CheckOverlapping(Shift shift, long employeeID)
        {
            await using var sqlConnection = new SqliteConnection(_connectionStringProvider.GetConnectionString());
            string SqlCheckOverlappingShiftsFormatted = string.Format(SqlCheckOverlappingShifts, shift.Start.ToString("yyyy-MM-dd HH:mm:ss.fff"), shift.End.ToString("yyyy-MM-dd HH:mm:ss.fff"), employeeID);
            var count = await sqlConnection.ExecuteScalarAsync<int>(SqlCheckOverlappingShiftsFormatted);

            return count > 0;
        }
    }
}

