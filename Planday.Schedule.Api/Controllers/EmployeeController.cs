using Microsoft.AspNetCore.Mvc;
using Planday.Schedule.Queries;

namespace Planday.Schedule.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeQuery _connection;
        public EmployeeController(IEmployeeQuery connection)
        {
            _connection = connection;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployeeById([FromRoute] long id)
        {
            if (id <= 0)
            {
                ModelState.AddModelError("id", "The 'id' must be greater than 0.");
                return BadRequest(ModelState);
            }

            var entity = await _connection.GetEmployeeById(id);

            if (entity == null)
                return NotFound();

            return Ok(entity);
        }

    }
}

