using Microsoft.AspNetCore.Mvc;
using Moq;
using Planday.Schedule;
using Planday.Schedule.Api.Controllers;
using Planday.Schedule.Infrastructure.Queries;

namespace SchedulingTest
{
    public class AssignamentUnitTest : BaseUnitTest
    {
        private AssignamentController _controller;
        private ShiftsQuery _shiftQuery;
        public AssignamentUnitTest()
        {
            var employeeMock = new Mock<EmployeeQuery>(_fakeConnectionProvider);
            var shiftsMock = new Mock<ShiftsQuery>(_fakeConnectionProvider);
            var assignamentMock = new Mock<AssignamentQuery>(_fakeConnectionProvider);
            _shiftQuery = new ShiftsQuery(_fakeConnectionProvider);
            _controller = new AssignamentController(assignamentMock.Object, employeeMock.Object, shiftsMock.Object);
        }

        [Fact]
        public async Task AddEmployeeToShift_EmployeeDoesNotExists_NotFoundObjectResult()
        {
            long shiftId = 1;
            long employeeId = long.MaxValue;
            int badRequestNumber = 404;

            var result = await _controller.AddEmployeeToShift(shiftId, employeeId);

            var actionResult = Assert.IsType<ActionResult<Shift>>(result);
            var request = actionResult.Result as NotFoundObjectResult;

            Assert.NotNull(request);
            Assert.Equal(badRequestNumber, request.StatusCode);
        }

        [Fact]
        public async Task AddEmployeeToShift_ShiftDoesntExists_NotFoundObjectResult()
        {
            long shiftId = long.MaxValue;
            long employeeId = 1;
            int badRequestNumber = 404;


            var result = await _controller.AddEmployeeToShift(shiftId, employeeId);

            var actionResult = Assert.IsType<ActionResult<Shift>>(result);
            var request = actionResult.Result as NotFoundObjectResult;

            Assert.NotNull(request);
            Assert.Equal(badRequestNumber, request.StatusCode);
        }

        [Fact]
        public async Task AddEmployeeToShift_ShiftAlreadyAssigned_BadRequest()
        {
            long shiftId = 1;
            long employeeId = 1;
            int badRequestNumber = 400;
            string errorMessage = "Shift is already assigned.";

            var result = await _controller.AddEmployeeToShift(shiftId, employeeId);

            var actionResult = Assert.IsType<ActionResult<Shift>>(result);
            var request = actionResult.Result as BadRequestObjectResult;

            Assert.NotNull(request);
            Assert.Equal(badRequestNumber, request.StatusCode);
            Assert.Equal(errorMessage, request.Value);
        }

        [Fact]
        public async Task AddEmployeeToShift_EmployeeOverlapped_BadRequest()
        {
            long shiftId = 5;
            long employeeId = 1;
            int badRequestNumber = 400;
            string errorMessage = "Employee has an overlap with the given shift.";

            var result = await _controller.AddEmployeeToShift(shiftId, employeeId);

            var actionResult = Assert.IsType<ActionResult<Shift>>(result);
            var request = actionResult.Result as BadRequestObjectResult;

            Assert.NotNull(request);
            Assert.Equal(badRequestNumber, request.StatusCode);
            Assert.Equal(errorMessage, request.Value);
        }

        [Fact]
        public async Task AddEmployeeToShift_ShouldWork_Ok()
        {
            long shiftId = 4;
            long employeeId = 2;

            var result = await _controller.AddEmployeeToShift(shiftId, employeeId);

            var actionResult = Assert.IsType<ActionResult<Shift>>(result);
            var request = actionResult.Result as OkObjectResult;

            Assert.NotNull(request);
            var model = Assert.IsAssignableFrom<Shift>(request.Value);

            Assert.Equal(shiftId, model.Id);
            Assert.Equal(employeeId, model.EmployeeId);

        }
    }
}
