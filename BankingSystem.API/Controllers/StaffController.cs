using BankingSystem.Application.DTOs;
using BankingSystem.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _service;

        public StaffController(IStaffService service)
        {
            _service = service;
        }

        // GET api/staff
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<List<StaffResponse>>> GetAll()
        {
            var staff = await _service.GetAllAsync();
            return Ok(staff);
        }

        // GET api/staff/1
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<StaffResponse>> GetById(int id)
        {
            var staff = await _service.GetByIdAsync(id);
            if (staff == null)
                return NotFound($"Staff with id {id} not found");
            return Ok(staff);
        }

        // POST api/staff/register
        [HttpPost("register")]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult<StaffResponse>> Register(
            RegisterStaffRequest request)
        {
            try
            {
                var staff = await _service.RegisterAsync(request);
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = staff.Id },
                    staff);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        // POST api/staff/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Login(
            StaffLoginRequest request)
        {
            try
            {
                var result = await _service.LoginAsync(request);
                if (result == null)
                    return Unauthorized("Invalid username or password");
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        // PUT api/staff/1
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<StaffResponse>> Update(
            int id,
            RegisterStaffRequest request)
        {
            try
            {
                var staff = await _service.UpdateAsync(id, request);
                if (staff == null)
                    return NotFound($"Staff with id {id} not found");
                return Ok(staff);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        // DELETE api/staff/1
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Deactivate(int id)
        {
            try
            {
                var result = await _service.DeactivateAsync(id);
                if (result == null)
                    return NotFound($"Staff with id {id} not found");
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}