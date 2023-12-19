using Dapper;
using Microsoft.Data.Sqlite;
using Planday.Schedule.Infrastructure.Providers.Interfaces;
using Planday.Schedule.Queries;

namespace Planday.Schedule.Infrastructure.Queries
{
    public class ShiftsQuery : IShiftsQuery
    {
        private readonly IConnectionStringProvider _connectionStringProvider;
        private readonly EmployeeQuery _employeeRepository;
        //SELECTS
        private const string SqlGetAllShifts = @"SELECT Id, EmployeeId, Start, End FROM Shift;";
        private const string SqlGetAllShiftsByEmployId = @"SELECT Id, EmployeeId, Start, End FROM Shift WHERE EmployeeId = {0};";
        private const string SqlGetShiftById = @"SELECT Id, EmployeeId, Start, End FROM Shift WHERE Id = {0};";
        private const string SqlGetLastInsert = @"SELECT * FROM Shift ORDER BY Id DESC LIMIT 1;";
        private const string SqlShiftAssignment = @"SELECT COUNT(*) FROM Shift WHERE Id = {0} AND EmployeeId > 0;";
        //INSERTS
        private const string SqlCreateOpenShift = @"INSERT INTO Shift (EmployeeId, Start, End) VALUES (-1, '{0}', '{1}');";
        private const string SqlCreateShift = @"INSERT INTO Shift (EmployeeId, Start, End) VALUES ({0}, '{1}', '{2}');";
        //UPDATES


        private record ShiftDto(long Id, long? EmployeeId, string Start, string End);

        public ShiftsQuery(IConnectionStringProvider connectionStringProvider)
        {
            _connectionStringProvider = connectionStringProvider;
            _employeeRepository = new EmployeeQuery(connectionStringProvider);
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
            string SqlGetShiftByIdFormated = string.Format(SqlGetShiftById, id);

            var shiftDtos = await sqlConnection.QueryAsync<ShiftDto>(SqlGetShiftByIdFormated);

            var shifts = shiftDtos.Select(x =>
                new Shift(x.Id, x.EmployeeId, DateTime.Parse(x.Start), DateTime.Parse(x.End)));

            return shifts.FirstOrDefault();
        }

        public async Task<IReadOnlyCollection<Shift>> GetAllShiftsByEmployeeId(long id)
        {
            await using var sqlConnection = new SqliteConnection(_connectionStringProvider.GetConnectionString());
            string SqlGetShiftByEmployeeIdFormated = string.Format(SqlGetAllShiftsByEmployId, id);

            var shiftDtos = await sqlConnection.QueryAsync<ShiftDto>(SqlGetShiftByEmployeeIdFormated);

            var shifts = shiftDtos.Select(x =>
                new Shift(x.Id, x.EmployeeId, DateTime.Parse(x.Start), DateTime.Parse(x.End)));

            return shifts.ToList();

        }

        public async Task<Shift> CreateOpenShift(Shift openShift)
        {
            await using var sqlConnection = new SqliteConnection(_connectionStringProvider.GetConnectionString());
            string insertSql = string.Format(SqlCreateOpenShift, openShift.Start.ToString("yyyy-MM-dd HH:mm:ss.fff"), openShift.End.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            await sqlConnection.ExecuteAsync(insertSql);

            var shiftDto = await sqlConnection.QueryFirstAsync<ShiftDto>(SqlGetLastInsert);

            if (shiftDto != null)
                return new Shift(shiftDto.Id, shiftDto.EmployeeId, DateTime.Parse(shiftDto.Start), DateTime.Parse(shiftDto.End));
            else
                throw new Exception("Error getting Last shift inserted.");
        }

        public async Task<bool> IsAssigned(long id)
        {
            await using var sqlConnection = new SqliteConnection(_connectionStringProvider.GetConnectionString());
            string SqlCheckShiftAssignaments = string.Format(SqlShiftAssignment, id);
            int count = await sqlConnection.ExecuteScalarAsync<int>(SqlCheckShiftAssignaments);
            return count > 0;
        }
    }
}

