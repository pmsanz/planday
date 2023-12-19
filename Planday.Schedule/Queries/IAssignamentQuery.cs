namespace Planday.Schedule.Queries
{
    public interface IAssignamentQuery
    {
        Task AddEmployeeToShift(long shiftId, long employeeId);
    }
}
