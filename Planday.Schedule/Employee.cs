namespace Planday.Schedule
{
    public class Employee
    {
        public Employee(long id, string name)
        {
            Id = id;
            Name = name;
        }

        public long Id { get; }
        public string Name { get; }
    }

    public class ExternalEmployee
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }
}

