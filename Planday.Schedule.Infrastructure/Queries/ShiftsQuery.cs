using Dapper;
using Microsoft.Data.Sqlite;
using Planday.Schedule.Infrastructure.Providers.Interfaces;
using Planday.Schedule.Queries;

namespace Planday.Schedule.Infrastructure.Queries
{
    public class ShiftsQuery : IShiftsQuery
    {
        private readonly IConnectionStringProvider _connectionStringProvider;

        private const string SqlGetAllShifts = @"SELECT Id, EmployeeId, Start, End FROM Shift;";
        private string SqlGetShiftById = @"SELECT Id, EmployeeId, Start, End FROM Shift WHERE Id = {0};";
        private record ShiftDto(long Id, long? EmployeeId, string Start, string End);

        public ShiftsQuery(IConnectionStringProvider connectionStringProvider)
        {
            _connectionStringProvider = connectionStringProvider;
        }

        public async Task<IReadOnlyCollection<Shift>> GetAllShifts()
        {
            await using var sqlConnection = new SqliteConnection(_connectionStringProvider.GetConnectionString());

            var shiftDtos = await sqlConnection.QueryAsync<ShiftDto>(SqlGetAllShifts);

            var shifts = shiftDtos.Select(x =>
                new Shift(x.Id, x.EmployeeId, DateTime.Parse(x.Start), DateTime.Parse(x.End)));

            return shifts.ToList();
        }

        public async Task<Shift?> GetShiftById(long id)
        {
            await using var sqlConnection = new SqliteConnection(_connectionStringProvider.GetConnectionString());
            SqlGetShiftById = string.Format(SqlGetShiftById, id);

            var shiftDtos = await sqlConnection.QueryAsync<ShiftDto>(SqlGetShiftById);

            var shifts = shiftDtos.Select(x =>
                new Shift(x.Id, x.EmployeeId, DateTime.Parse(x.Start), DateTime.Parse(x.End)));

            return shifts.FirstOrDefault();
        }
    }
}

