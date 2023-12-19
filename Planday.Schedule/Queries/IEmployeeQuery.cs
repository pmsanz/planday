namespace Planday.Schedule.Queries
{
    public interface IEmployeeQuery
    {
        Task<Employee?> GetEmployeeById(long id);
        Task<bool> CheckOverlapping(Shift shift, long employeeId);
    }
}

