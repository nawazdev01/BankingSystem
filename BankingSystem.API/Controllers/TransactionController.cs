using BankingSystem.Application.DTOs;
using BankingSystem.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _service;

        public TransactionController(ITransactionService service)
        {
            _service = service;
        }

        // POST api/transaction/deposit
        [HttpPost("deposit")]
        public async Task<ActionResult<TransactionResponse>> Deposit(
            DepositRequest request)
        {
            try
            {
                var transaction = await _service.DepositAsync(request);
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

        // POST api/transaction/withdraw
        [HttpPost("withdraw")]
        public async Task<ActionResult<TransactionResponse>> Withdraw(
            WithdrawRequest request)
        {
            try
            {
                var transaction = await _service.WithdrawAsync(request);
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

        // POST api/transaction/transfer
        [HttpPost("transfer")]
        public async Task<ActionResult<TransactionResponse>> Transfer(
            TransferRequest request)
        {
            try
            {
                var transaction = await _service.TransferAsync(request);
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

        // GET api/transaction/account/1
        [HttpGet("account/{accountId}")]
        public async Task<ActionResult<List<TransactionResponse>>> GetByAccount(
            int accountId)
        {
            var transactions = await _service.GetByAccountAsync(accountId);
            if (transactions.Count == 0)
                return NotFound(
                    $"No transactions found for account {accountId}");
            return Ok(transactions);
        }

        // GET api/transaction/customer/1
        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<List<TransactionResponse>>> GetByCustomer(
            int customerId)
        {
            var transactions = await _service.GetByCustomerAsync(customerId);
            if (transactions.Count == 0)
                return NotFound(
                    $"No transactions found for customer {customerId}");
            return Ok(transactions);
        }

        // GET api/transaction/reference/TXN123
        [HttpGet("reference/{reference}")]
        public async Task<ActionResult<TransactionResponse>> GetByReference(
            string reference)
        {
            var transaction = await _service.GetByReferenceAsync(reference);
            if (transaction == null)
                return NotFound(
                    $"Transaction with reference {reference} not found");
            return Ok(transaction);
        }

        // GET api/transaction/account/1/daterange?from=2024-01-01&to=2024-12-31
        [HttpGet("account/{accountId}/daterange")]
        public async Task<ActionResult<List<TransactionResponse>>> GetByDateRange(
            int accountId,
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            if (from > to)
                return BadRequest("From date must be before To date");

            var transactions = await _service
                .GetByDateRangeAsync(accountId, from, to);

            if (transactions.Count == 0)
                return NotFound(
                    $"No transactions found for account {accountId} " +
                    $"between {from:yyyy-MM-dd} and {to:yyyy-MM-dd}");

            return Ok(transactions);
        }
    }
}