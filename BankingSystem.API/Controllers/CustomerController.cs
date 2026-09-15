using BankingSystem.Application.DTOs;
using BankingSystem.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _service;

        public CustomerController(ICustomerService service)
        {
            _service = service;
        }

        // GET api/customer
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Teller")]
        public async Task<ActionResult<List<CustomerResponse>>> GetAll()
        {
            var customers = await _service.GetAllAsync();
            return Ok(customers);
        }

        // GET api/customer/1
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<CustomerResponse>> GetById(int id)
        {
            var customer = await _service.GetByIdAsync(id);
            if (customer == null)
                return NotFound($"Customer with id {id} not found");
            return Ok(customer);
        }

        // POST api/customer/register
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<CustomerResponse>> Register(
            RegisterCustomerRequest request)
        {
            try
            {
                var customer = await _service.RegisterAsync(request);
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = customer.Id },
                    customer);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        // POST api/customer/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Login(
            CustomerLoginRequest request)
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

        // PUT api/customer/1
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<CustomerResponse>> Update(
            int id,
            UpdateCustomerRequest request)
        {
            try
            {
                var customer = await _service.UpdateAsync(id, request);
                if (customer == null)
                    return NotFound($"Customer with id {id} not found");
                return Ok(customer);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE api/customer/1
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult> Deactivate(int id)
        {
            try
            {
                var result = await _service.DeactivateAsync(id);
                if (result == null)
                    return NotFound($"Customer with id {id} not found");
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PATCH api/customer/1/verify-kyc
        [HttpPatch("{id}/verify-kyc")]
        [Authorize(Roles = "Admin,Manager,Teller")]
        public async Task<ActionResult> VerifyKYC(int id)
        {
            try
            {
                var result = await _service.VerifyKYCAsync(id);
                if (result == null)
                    return NotFound($"Customer with id {id} not found");
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}