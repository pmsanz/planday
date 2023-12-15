using Microsoft.AspNetCore.Mvc;
using Moq;
using Planday.Schedule;
using Planday.Schedule.Api.Controllers;
using Planday.Schedule.Infrastructure.Providers.Interfaces;
using Planday.Schedule.Infrastructure.Queries;

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

    public class ShiftsUnitTest
    {
        private ShiftController _controller;
        private List<Shift> _expectedShifts = new List<Shift>();
        public ShiftsUnitTest()
        {
            FakeConnectionProvider fake = new FakeConnectionProvider();
            var mockRepo = new Mock<ShiftsQuery>(fake);
            _controller = new ShiftController(mockRepo.Object);

            _expectedShifts.Add(new Shift(id: 1, 1, new DateTime(2022, 6, 17, 12, 0, 0), new DateTime(2022, 6, 17, 17, 0, 0)));
            _expectedShifts.Add(new Shift(id: 2, 2, new DateTime(2022, 6, 17, 9, 0, 0), new DateTime(2022, 6, 17, 15, 0, 0)));
        }

        [Fact]
        public async Task GetAllShifts_ShouldReturnListReadOnly_True()
        {
            var result = await _controller.GetAllShifts();

            var actionResult = Assert.IsType<ActionResult<IReadOnlyCollection<Shift>>>(result);
            var okResult = actionResult.Result as OkObjectResult;

            Assert.NotNull(okResult);
            var model = Assert.IsAssignableFrom<IReadOnlyCollection<Shift>>(okResult.Value);
            AssertShiftCollectionsAreEqual(_expectedShifts, model);
        }

        [Fact]
        public async Task GetShiftById_ShouldReturnId1_True()
        {
            long shiftId = 1;
            var result = await _controller.GetShiftById(shiftId);

            var actionResult = Assert.IsType<ActionResult<Shift>>(result);
            var okResult = actionResult.Result as OkObjectResult;

            Assert.NotNull(okResult);
            var actual = Assert.IsAssignableFrom<Shift>(okResult.Value);

            Assert.Equal(_expectedShifts.ElementAt(0).Id, actual.Id);
            Assert.Equal(_expectedShifts.ElementAt(0).EmployeeId, actual.EmployeeId);
            Assert.Equal(_expectedShifts.ElementAt(0).Start, actual.Start);
            Assert.Equal(_expectedShifts.ElementAt(0).End, actual.End);
        }
        [Fact]
        public async Task GetShiftById_ShouldReturnId2_True()
        {
            long shiftId = 2;
            var result = await _controller.GetShiftById(shiftId);

            var actionResult = Assert.IsType<ActionResult<Shift>>(result);
            var okResult = actionResult.Result as OkObjectResult;

            Assert.NotNull(okResult);
            var actual = Assert.IsAssignableFrom<Shift>(okResult.Value);

            Assert.Equal(_expectedShifts.ElementAt(1).Id, actual.Id);
            Assert.Equal(_expectedShifts.ElementAt(1).EmployeeId, actual.EmployeeId);
            Assert.Equal(_expectedShifts.ElementAt(1).Start, actual.Start);
            Assert.Equal(_expectedShifts.ElementAt(1).End, actual.End);
        }
        [Fact]
        public async Task GetShiftById_ShouldReturnNotFound_True()
        {
            long shiftId = 3;
            int notFoundCode = 404;
            var result = await _controller.GetShiftById(shiftId);

            var actionResult = Assert.IsType<ActionResult<Shift>>(result);
            var notFound = actionResult.Result as NotFoundResult;
            Assert.NotNull(notFound);
            Assert.Equal(notFoundCode, notFound.StatusCode);
        }
        [Fact]
        public async Task GetShiftById_BadRequest_True()
        {
            long shiftId = -1;
            int badRequestCode = 400;
            var result = await _controller.GetShiftById(shiftId);

            var actionResult = Assert.IsType<ActionResult<Shift>>(result);
            var badRequest = actionResult.Result as BadRequestObjectResult;

            Assert.NotNull(badRequest);
            Assert.Equal(badRequestCode, badRequest.StatusCode);
        }

        private static void AssertShiftCollectionsAreEqual(
       List<Shift> expected,
       IReadOnlyCollection<Shift>? actual)
        {
            if (expected == null || actual == null)
                Assert.Fail("Shift lists cannot be null in method AssertShiftCollectionsAreEqual.");

            Assert.Equal(expected.Count, actual.Count);

            for (int i = 0; i < expected.Count; i++)
            {
                Assert.Equal(expected.ElementAt(i).Id, actual.ElementAt(i).Id);
                Assert.Equal(expected.ElementAt(i).EmployeeId, actual.ElementAt(i).EmployeeId);
                Assert.Equal(expected.ElementAt(i).Start, actual.ElementAt(i).Start);
                Assert.Equal(expected.ElementAt(i).End, actual.ElementAt(i).End);
            }
        }
    }
}