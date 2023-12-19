namespace Planday.Schedule.Queries
{
    public interface IShiftsQuery
    {
        Task<IReadOnlyCollection<Shift>> GetAllShifts();
        Task<IReadOnlyCollection<Shift>> GetAllShiftsByEmployeeId(long id);
        Task<Shift?> GetShiftById(long id);
        Task<Shift> CreateOpenShift(Shift id);
        Task<bool> IsAssigned(long id);

    }
}

