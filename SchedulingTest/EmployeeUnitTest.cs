using Microsoft.AspNetCore.Mvc;
using Moq;
using Planday.Schedule;
using Planday.Schedule.Api.Controllers;
using Planday.Schedule.Infrastructure.Queries;

namespace SchedulingTest
{
    public class EmployeeUnitTest : BaseUnitTest
    {
        private EmployeeController _controller;
        private EmployeeQuery _employeeQuery;
        private List<Employee> _expectedEmployees;
        public EmployeeUnitTest()
        {
            var mockRepo = new Mock<EmployeeQuery>(_fakeConnectionProvider);
            _controller = new EmployeeController(mockRepo.Object);
            _employeeQuery = mockRepo.Object;
            _expectedEmployees = new List<Employee>();
            _expectedEmployees.Add(new Employee(1, "John Doe"));
            _expectedEmployees.Add(new Employee(2, "Jane Doe"));
        }


        [Fact]
        public async Task GetEmployeeById_ShouldReturnId1_True()
        {
            long shiftId = 1;
            var result = await _controller.GetEmployeeById(shiftId);

            var actionResult = Assert.IsType<ActionResult<Employee>>(result);
            var okResult = actionResult.Result as OkObjectResult;

            Assert.NotNull(okResult);
            var actual = Assert.IsAssignableFrom<Employee>(okResult.Value);

            Assert.Equal(_expectedEmployees.ElementAt(0).Id, actual.Id);
            Assert.Equal(_expectedEmployees.ElementAt(0).Name, actual.Name);
        }
        [Fact]
        public async Task GetEmployeeById_ShouldReturnId2_True()
        {
            long shiftId = 2;
            var result = await _controller.GetEmployeeById(shiftId);

            var actionResult = Assert.IsType<ActionResult<Employee>>(result);
            var okResult = actionResult.Result as OkObjectResult;

            Assert.NotNull(okResult);
            var actual = Assert.IsAssignableFrom<Employee>(okResult.Value);

            Assert.Equal(_expectedEmployees.ElementAt(1).Id, actual.Id);
            Assert.Equal(_expectedEmployees.ElementAt(1).Name, actual.Name);
        }

        [Fact]
        public async Task CheckOverlapping_True()
        {
            long shiftId = 3;
            long employeeId = 1;

            Shift shiftFake = new Shift(shiftId, employeeId, new DateTime(2022, 6, 17, 16, 0, 0), new DateTime(2022, 6, 17, 20, 0, 0));
            var result = await _employeeQuery.CheckOverlapping(shiftFake, employeeId);

            Assert.True(result);
        }

        [Fact]
        public async Task CheckOverlapping_False()
        {
            long shiftId = 3;
            long employeeId = 1;

            Shift shiftFake = new Shift(shiftId, employeeId, new DateTime(2022, 6, 17, 17, 1, 0), new DateTime(2022, 6, 17, 20, 0, 0));
            var result = await _employeeQuery.CheckOverlapping(shiftFake, employeeId);

            Assert.False(result);
        }




    }
}
