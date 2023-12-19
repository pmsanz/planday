using Microsoft.AspNetCore.Mvc;
using Planday.Schedule.Queries;

namespace Planday.Schedule.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AssignamentController : ControllerBase
    {

        private readonly IAssignamentQuery _connectionAssignament;
        private readonly IShiftsQuery _connectionShifts;
        private readonly IEmployeeQuery _connectionEmployee;
        public AssignamentController(IAssignamentQuery assignament, IEmployeeQuery employee, IShiftsQuery shift)
        {
            _connectionAssignament = assignament;
            _connectionShifts = shift;
            _connectionEmployee = employee;
        }


        [HttpPost("{shiftId}/{employeeId}")]
        public async Task<ActionResult<Shift>> AddEmployeeToShift([FromRoute] long shiftId, [FromRoute] long employeeId)
        {
            if (shiftId < 0 || employeeId < 0)
                return BadRequest("Ids must have a value grater than 0");

            var employee = await _connectionEmployee.GetEmployeeById(employeeId);
            var shift = await _connectionShifts.GetShiftById(shiftId);

            if (employee == null)
                return NotFound("Employee doesn't exists.");

            if (shift == null)
                return NotFound("Shift doesn't exists.");

            var isAssigned = await _connectionShifts.IsAssigned(shift.Id);

            if (isAssigned)
                return BadRequest("Shift is already assigned.");

            var isOverlapping = await _connectionEmployee.CheckOverlapping(shift, employeeId);

            if (isOverlapping)
                return BadRequest("Employee has an overlap with the given shift.");

            await _connectionAssignament.AddEmployeeToShift(shiftId, employeeId);
            var shiftUpdated = await _connectionShifts.GetShiftById(shiftId);
            return Ok(shiftUpdated);

        }



    }
}


