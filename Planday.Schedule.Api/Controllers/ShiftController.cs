using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Planday.Schedule.Queries;
using System.Net.Http.Headers;

namespace Planday.Schedule.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShiftController : ControllerBase
    {

        private readonly IShiftsQuery _connection;
        private readonly IConfiguration _appSettings;
        public ShiftController(IShiftsQuery connection, IConfiguration appSettings)
        {
            _connection = connection;
            _appSettings = appSettings;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyCollection<Shift>>> GetAllShifts()
        {
            return Ok(await _connection.GetAllShifts());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IReadOnlyCollection<Shift>>> GetAllShiftsByEmployeeId([FromRoute] long id)
        {
            if (id <= 0)
            {
                ModelState.AddModelError("id", "The 'id' must be greater than 0.");
                return BadRequest(ModelState);
            }

            var entity = await _connection.GetAllShiftsByEmployeeId(id);

            if (entity == null)
                return NotFound();

            return Ok(entity);
        }

        [HttpGet("{id}/details")]
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

            string email = "Error getting email.";

            try
            {
                var employeeAction = await GetExternalEmployeeByExternalAPI(entity.Id);
                if (employeeAction != null)
                    email = employeeAction.Email;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error fetching Employee from external API, message: " + e.Message);
            }

            entity.EmployeeEmail = email;
            return Ok(entity);
        }

        [HttpGet("{id}/external-api-details")]
        public async Task<ExternalEmployee?> GetExternalEmployeeByExternalAPI(long id)
        {
            try
            {

                var authToken = _appSettings["AuthorizationTokens:PlanDayExternalEmployeeAPI"];
                var planDayEndpoint = _appSettings["ExternalEndpoints:PlanDayEndpoint"];
                string endpointAndParam = @"/employee/{0}";
                endpointAndParam = string.Format(endpointAndParam, id);
                using var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(planDayEndpoint);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authToken);
                HttpResponseMessage response = await httpClient.GetAsync(endpointAndParam);
                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var employee = JsonConvert.DeserializeObject<ExternalEmployee>(jsonContent);
                    return employee;
                }
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Error getting data from external API", ex);
            }
            return null;
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
        [HttpGet("{id}/isassigned")]
        public async Task<ActionResult<bool>> IsAssigned([FromRoute] long id)
        {
            if (id <= 0)
            {
                ModelState.AddModelError("id", "The 'id' must be greater than 0.");
                return BadRequest(ModelState);
            }
            var isAssigned = await _connection.IsAssigned(id);
            return Ok(isAssigned);
        }

    }
}

