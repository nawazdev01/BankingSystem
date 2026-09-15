using BankingSystem.Application.DTOs;
using BankingSystem.Application.Services;
using BankingSystem.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankingSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LoanController : ControllerBase
    {
        private readonly ILoanService _service;

        public LoanController(ILoanService service)
        {
            _service = service;
        }

        // GET api/loan
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,LoanOfficer")]
        public async Task<ActionResult<List<LoanResponse>>> GetAll()
        {
            var loans = await _service.GetAllAsync();
            return Ok(loans);
        }

        // GET api/loan/1
        [HttpGet("{id}")]
        public async Task<ActionResult<LoanResponse>> GetById(int id)
        {
            var loan = await _service.GetByIdAsync(id);
            if (loan == null)
                return NotFound($"Loan with id {id} not found");
            return Ok(loan);
        }

        // GET api/loan/customer/1
        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<List<LoanResponse>>> GetByCustomer(
            int customerId)
        {
            var loans = await _service.GetByCustomerAsync(customerId);
            if (loans.Count == 0)
                return NotFound($"No loans found for customer {customerId}");
            return Ok(loans);
        }

        // GET api/loan/status/Pending
        [HttpGet("status/{status}")]
        [Authorize(Roles = "Admin,Manager,LoanOfficer")]
        public async Task<ActionResult<List<LoanResponse>>> GetByStatus(
            LoanStatus status)
        {
            var loans = await _service.GetByStatusAsync(status);
            if (loans.Count == 0)
                return NotFound($"No loans found with status {status}");
            return Ok(loans);
        }

        // POST api/loan/apply
        [HttpPost("apply")]
        public async Task<ActionResult<LoanResponse>> Apply(
            ApplyLoanRequest request)
        {
            try
            {
                var loan = await _service.ApplyAsync(request);
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = loan.Id },
                    loan);
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

        // PATCH api/loan/1/approve
        [HttpPatch("{id}/approve")]
        [Authorize(Roles = "Admin,Manager,LoanOfficer")]
        public async Task<ActionResult<LoanResponse>> Approve(
            int id,
            ApproveLoanRequest request)
        {
            try
            {
                // Get staff id from JWT token claims
                var staffIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (staffIdClaim == null)
                    return Unauthorized("Staff id not found in token");

                int staffId = int.Parse(staffIdClaim);
                var loan    = await _service.ApproveAsync(id, staffId, request);

                if (loan == null)
                    return NotFound($"Loan with id {id} not found");

                return Ok(loan);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PATCH api/loan/1/reject
        [HttpPatch("{id}/reject")]
        [Authorize(Roles = "Admin,Manager,LoanOfficer")]
        public async Task<ActionResult<LoanResponse>> Reject(
            int id,
            RejectLoanRequest request)
        {
            try
            {
                var staffIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (staffIdClaim == null)
                    return Unauthorized("Staff id not found in token");

                int staffId = int.Parse(staffIdClaim);
                var loan    = await _service.RejectAsync(id, staffId, request);

                if (loan == null)
                    return NotFound($"Loan with id {id} not found");

                return Ok(loan);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PATCH api/loan/1/disburse
        [HttpPatch("{id}/disburse")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<LoanResponse>> Disburse(int id)
        {
            try
            {
                var loan = await _service.DisburseAsync(id);
                if (loan == null)
                    return NotFound($"Loan with id {id} not found");
                return Ok(loan);
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

        // POST api/loan/repay
        [HttpPost("repay")]
        public async Task<ActionResult<TransactionResponse>> Repay(
            LoanRepaymentRequest request)
        {
            try
            {
                var transaction = await _service.RepayAsync(request);
                return Ok(transaction);
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
    }
}