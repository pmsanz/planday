namespace Planday.Schedule.Queries
{
    public interface IShiftsQuery
    {
        Task<IReadOnlyCollection<Shift>> GetAllShifts();
        Task<Shift?> GetShiftById(long id);
    }
}

