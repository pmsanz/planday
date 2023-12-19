using Planday.Schedule.Infrastructure.Providers.Interfaces;

namespace SchedulingTest
{
    public class FakeConnectionProvider : IConnectionStringProvider
    {
        public FakeConnectionProvider()
        {

        }
        public string GetConnectionString()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string dbName = "planday-schedule.db";
            string connectionString = string.Format("Data Source={0};", Path.Combine(currentDirectory, dbName));
            return connectionString;
        }
    }
    public class BaseUnitTest
    {
        internal FakeConnectionProvider _fakeConnectionProvider;
        public BaseUnitTest()
        {
            _fakeConnectionProvider = new FakeConnectionProvider();
        }

        public void RestoreTestingBD()
        {

            try
            {
                string sourceFilePath = Path.Combine("..", "..", "..", "Resources", "planday-schedule.db");
                string fullSourcePath = Path.GetFullPath(sourceFilePath);
                string currentDirectory = Directory.GetCurrentDirectory();
                string destinationFilePath = Path.Combine(currentDirectory, "planday-schedule.db");
                File.Copy(fullSourcePath, destinationFilePath, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

        }

    }
}
