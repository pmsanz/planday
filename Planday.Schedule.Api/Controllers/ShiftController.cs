using Microsoft.AspNetCore.Mvc;
using Planday.Schedule.Queries;

namespace Planday.Schedule.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShiftController : ControllerBase
    {
        private readonly IShiftsQuery _connection;
        public ShiftController(IShiftsQuery connection)
        {
            _connection = connection;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyCollection<Shift>>> GetAllShifts()
        {
            return Ok(await _connection.GetAllShifts());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Shift>> GetShiftById([FromRoute] long id)
        {
            if (id <= 0)
            {
                ModelState.AddModelError("id", "The 'id' must be greater than 0.");
                return BadRequest(ModelState);
            }

            var entity = await _connection.GetShiftById(id);

            if (entity == null)
                return NotFound();

            return Ok(entity);
        }

        [HttpPost]
        public async Task<ActionResult<Shift>> CreateOpenShift([FromRoute] Shift shift)
        {

            if (shift.Start > shift.End)
            {
                return BadRequest("Start time cannot be greater than end time.");
            }

            if (shift.Start.Date != shift.End.Date)
            {
                return BadRequest("Start and end time should be on the same day.");
            }

            var entity = await _connection.CreateOpenShift(shift);

            return Ok(entity);
        }
    }
}

