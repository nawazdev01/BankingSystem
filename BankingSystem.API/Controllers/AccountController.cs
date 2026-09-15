using BankingSystem.Application.DTOs;
using BankingSystem.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _service;

        public AccountController(IAccountService service)
        {
            _service = service;
        }

        // GET api/account
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Teller")]
        public async Task<ActionResult<List<AccountResponse>>> GetAll()
        {
            var accounts = await _service.GetAllAsync();
            return Ok(accounts);
        }

        // GET api/account/1
        [HttpGet("{id}")]
        public async Task<ActionResult<AccountResponse>> GetById(int id)
        {
            var account = await _service.GetByIdAsync(id);
            if (account == null)
                return NotFound($"Account with id {id} not found");
            return Ok(account);
        }

        // GET api/account/customer/1
        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<List<AccountResponse>>> GetByCustomer(
            int customerId)
        {
            var accounts = await _service.GetByCustomerAsync(customerId);
            if (accounts.Count == 0)
                return NotFound(
                    $"No accounts found for customer {customerId}");
            return Ok(accounts);
        }

        // POST api/account
        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Teller")]
        public async Task<ActionResult<AccountResponse>> Create(
            CreateAccountRequest request)
        {
            try
            {
                var account = await _service.CreateAsync(request);
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = account.Id },
                    account);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PATCH api/account/1/freeze
        [HttpPatch("{id}/freeze")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult> Freeze(int id)
        {
            try
            {
                var result = await _service.FreezeAsync(id);
                if (result == null)
                    return NotFound($"Account with id {id} not found");
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PATCH api/account/1/unfreeze
        [HttpPatch("{id}/unfreeze")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult> Unfreeze(int id)
        {
            try
            {
                var result = await _service.UnfreezeAsync(id);
                if (result == null)
                    return NotFound($"Account with id {id} not found");
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PATCH api/account/1/close
        [HttpPatch("{id}/close")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult> Close(int id)
        {
            try
            {
                var result = await _service.CloseAsync(id);
                if (result == null)
                    return NotFound($"Account with id {id} not found");
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}