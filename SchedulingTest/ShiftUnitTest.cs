using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Planday.Schedule;
using Planday.Schedule.Api.Controllers;
using Planday.Schedule.Infrastructure.Queries;

namespace SchedulingTest
{
    public class ShiftUnitTest : BaseUnitTest
    {
        private ShiftController _controller;
        private List<Shift> _expectedShifts = new List<Shift>();

        public ShiftUnitTest()
        {
            var mockRepo = new Mock<ShiftsQuery>(_fakeConnectionProvider);
            var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
            _controller = new ShiftController(mockRepo.Object, configuration);

            _expectedShifts.Add(new Shift(id: 1, 1, new DateTime(2022, 6, 17, 12, 0, 0), new DateTime(2022, 6, 17, 17, 0, 0)));
            _expectedShifts.Add(new Shift(id: 2, 2, new DateTime(2022, 6, 17, 9, 0, 0), new DateTime(2022, 6, 17, 15, 0, 0)));
            _expectedShifts.Add(new Shift(id: 3, -1, new DateTime(2023, 12, 23, 9, 0, 0), new DateTime(2023, 12, 23, 17, 0, 0)));
            _expectedShifts.Add(new Shift(id: 4, -1, new DateTime(2023, 12, 23, 17, 0, 0), new DateTime(2023, 12, 24, 9, 0, 0)));
            _expectedShifts.Add(new Shift(id: 5, -1, new DateTime(2022, 6, 17, 12, 0, 0), new DateTime(2022, 6, 17, 17, 0, 0)));
        }

        [Fact]
        public async Task GetAllShifts_ShouldReturnListReadOnly_True()
        {
            RestoreTestingBD();
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
            Assert.Equal("john@doe.com", actual.EmployeeEmail);
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
            Assert.Equal("jane@doe.com", actual.EmployeeEmail);

        }

        [Fact]
        public async Task GetExternalEmployeeByExternalAPI_ShouldReturnId2_True()
        {
            long shiftId = 2;
            var result = await _controller.GetExternalEmployeeByExternalAPI(shiftId);

            var externalEmployee = Assert.IsType<ExternalEmployee>(result);
            var actual = externalEmployee as ExternalEmployee;

            Assert.NotNull(actual);
            Assert.Equal("jane@doe.com", actual.Email);
            Assert.Equal("Jane Doe", actual.Name);

        }

        [Fact]
        public async Task GetExternalEmployeeApiByExternalAPI_WhenBadRequestResponse_ShouldThrowHttpRequestException_WithExpectedMessage()
        {
            long shiftId = 3;

            var exception = await Assert.ThrowsAsync<Exception>(() => _controller.GetExternalEmployeeByExternalAPI(shiftId));
            Assert.NotNull(exception);
            Assert.Contains("Error getting data from external API", exception.Message);


        }

        [Fact]
        public async Task GetShiftById_ShouldReturnNotFound_True()
        {
            long shiftId = long.MaxValue;
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

        [Fact]
        public async Task GetShiftsByEmployeeId_ShouldReturnListReadOnly_True()
        {
            long employeeId = 1;
            var result = await _controller.GetAllShiftsByEmployeeId(employeeId);

            var actionResult = Assert.IsType<ActionResult<IReadOnlyCollection<Shift>>>(result);
            var okResult = actionResult.Result as OkObjectResult;

            Assert.NotNull(okResult);
            var model = Assert.IsAssignableFrom<IReadOnlyCollection<Shift>>(okResult.Value);
            AssertShiftCollectionsAreEqual(_expectedShifts.Where(x => x.EmployeeId == employeeId).ToList(), model);
        }

        [Fact]
        public async Task CreateOpenShift_Ok_True()
        {
            DateTime start = new DateTime(2023, 12, 23, 9, 0, 0);
            DateTime end = new DateTime(2023, 12, 23, 17, 0, 0);
            Shift shift = new Shift(0, null, start, end);
            var result = await _controller.CreateOpenShift(shift);

            var actionResult = Assert.IsType<ActionResult<Shift>>(result);
            var okResult = actionResult.Result as OkObjectResult;

            Assert.NotNull(okResult);
            var actual = Assert.IsAssignableFrom<Shift>(okResult.Value);
            Assert.Equal(start, actual.Start);
            Assert.Equal(end, actual.End);
        }

        [Fact]
        public async Task CreateOpenShift_NotSameDay_True()
        {
            int badRequestCode = 400;
            DateTime start = new DateTime(2023, 12, 23, 9, 0, 0);
            DateTime end = new DateTime(2023, 12, 24, 17, 0, 0);
            Shift shift = new Shift(0, null, start, end);
            var result = await _controller.CreateOpenShift(shift);

            var actionResult = Assert.IsType<ActionResult<Shift>>(result);
            var badRequest = actionResult.Result as BadRequestObjectResult;

            Assert.NotNull(badRequest);
            Assert.Equal(badRequestCode, badRequest.StatusCode);
            Assert.Equal("Start and end time should be on the same day.", badRequest.Value);
        }

        [Fact]
        public async Task CreateOpenShift_StartDateIsGreaterThanEndDate_True()
        {
            int badRequestCode = 400;
            DateTime start = new DateTime(2023, 12, 24, 13, 0, 0);
            DateTime end = new DateTime(2023, 12, 24, 9, 0, 0);
            Shift shift = new Shift(0, null, start, end);
            var result = await _controller.CreateOpenShift(shift);

            var actionResult = Assert.IsType<ActionResult<Shift>>(result);
            var badRequest = actionResult.Result as BadRequestObjectResult;

            Assert.NotNull(badRequest);
            Assert.Equal(badRequestCode, badRequest.StatusCode);
            Assert.Equal("Start time cannot be greater than end time.", badRequest.Value);
        }



        [Fact]
        public async Task CreateOpenShift_ShouldReturnBadRequest_StartDateGreaterThanEndDate()
        {
            long shiftId = -1;
            int badRequestCode = 400;
            var result = await _controller.GetShiftById(shiftId);

            var actionResult = Assert.IsType<ActionResult<Shift>>(result);
            var badRequest = actionResult.Result as BadRequestObjectResult;

            Assert.NotNull(badRequest);
            Assert.Equal(badRequestCode, badRequest.StatusCode);
        }

        [Fact]
        public async Task CreateOpenShift_ShouldReturnBadRequest_NotSameDay()
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