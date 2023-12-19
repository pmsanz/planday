using Dapper;
using Microsoft.Data.Sqlite;
using Planday.Schedule.Infrastructure.Providers.Interfaces;
using Planday.Schedule.Queries;

namespace Planday.Schedule.Infrastructure.Queries
{
    public class AssignamentQuery : IAssignamentQuery
    {
        private readonly IConnectionStringProvider _connectionStringProvider;

        private const string SqlUpdateShift = @"UPDATE Shift SET EmployeeId = {0} WHERE Id = {1};";
        private record EmployeeDto(long Id, string Name);

        public AssignamentQuery(IConnectionStringProvider connectionStringProvider)
        {
            _connectionStringProvider = connectionStringProvider;
        }

        public async Task AddEmployeeToShift(long shiftId, long employeeId)
        {
            if (shiftId < 1)
                throw new Exception($"Shift should be greater than 0.");

            if (employeeId < 1)
                throw new Exception($"Employee should be greater than 0.");

            await UpdateShiftInDatabase(shiftId, employeeId);
        }

        private async Task UpdateShiftInDatabase(long shiftId, long employeeId)
        {
            await using var sqlConnection = new SqliteConnection(_connectionStringProvider.GetConnectionString());
            string updateSql = string.Format(SqlUpdateShift, employeeId, shiftId);
            await sqlConnection.ExecuteAsync(updateSql);
        }

    }
}
