using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InvoiceApi.Data;
using InvoiceApi.Models;

namespace InvoiceApi.Controllers
{
    /// <summary>
    /// Invoice API Controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly InvoiceDbContext _context;

        public InvoiceController(InvoiceDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all invoices
        /// </summary>
        /// <returns>List of invoices</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<object>>> GetInvoices()
        {
            var invoices = await _context.Invoices
                .Include(i => i.Items)
                .ToListAsync();

            if (invoices == null || invoices.Count == 0)
            {
                return NotFound(new { message = "No invoices found" });
            }

            var result = invoices.Select(inv => new
            {
                invoiceId = inv.InvoiceId,
                customerName = inv.CustomerName,
                invoiceDate = inv.InvoiceDate,
                totalAmount = inv.TotalAmount,
                items = inv.Items.Select(item => new
                {
                    itemId = item.ItemId,
                    name = item.Name,
                    price = item.Price,
                    quantity = item.Quantity
                })
            });

            return Ok(result);
        }

        /// <summary>
        /// Get invoice by ID
        /// </summary>
        /// <param name="id">Invoice ID</param>
        /// <returns>Invoice details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<object>> GetInvoice(int id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Items)
                .FirstOrDefaultAsync(i => i.InvoiceId == id);

            if (invoice == null)
            {
                return NotFound(new { message = $"Invoice with ID {id} not found" });
            }

            var result = new
            {
                invoiceId = invoice.InvoiceId,
                customerName = invoice.CustomerName,
                invoiceDate = invoice.InvoiceDate,
                totalAmount = invoice.TotalAmount,
                items = invoice.Items.Select(item => new
                {
                    itemId = item.ItemId,
                    name = item.Name,
                    price = item.Price,
                    quantity = item.Quantity
                })
            };

            return Ok(result);
        }

        /// <summary>
        /// Create a new invoice
        /// </summary>
        /// <param name="invoice">Invoice data</param>
        /// <returns>Created invoice</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<object>> CreateInvoice([FromBody] CreateInvoiceRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.CustomerName))
            {
                return BadRequest(new { message = "Customer name is required" });
            }

            var invoice = new Invoice
            {
                CustomerName = request.CustomerName,
                InvoiceDate = DateTime.UtcNow,
                TotalAmount = 0
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetInvoice), new { id = invoice.InvoiceId }, new
            {
                invoiceId = invoice.InvoiceId,
                customerName = invoice.CustomerName,
                invoiceDate = invoice.InvoiceDate,
                totalAmount = invoice.TotalAmount,
                items = new List<object>()
            });
        }

        /// <summary>
        /// Add item to invoice
        /// </summary>
        /// <param name="invoiceId">Invoice ID</param>
        /// <param name="request">Item data</param>
        /// <returns>Updated invoice</returns>
        [HttpPost("{invoiceId}/items")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<object>> AddItemToInvoice(int invoiceId, [FromBody] AddItemRequest request)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Items)
                .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);

            if (invoice == null)
            {
                return NotFound(new { message = $"Invoice with ID {invoiceId} not found" });
            }

            if (string.IsNullOrWhiteSpace(request.Name) || request.Price <= 0)
            {
                return BadRequest(new { message = "Item name and price are required" });
            }

            var item = new InvoiceItem
            {
                InvoiceId = invoiceId,
                Name = request.Name,
                Price = request.Price,
                Quantity = request.Quantity > 0 ? request.Quantity : 1
            };

            _context.InvoiceItems.Add(item);

            // Update total amount
            invoice.TotalAmount += item.Price * item.Quantity;

            await _context.SaveChangesAsync();

            var result = new
            {
                invoiceId = invoice.InvoiceId,
                customerName = invoice.CustomerName,
                invoiceDate = invoice.InvoiceDate,
                totalAmount = invoice.TotalAmount,
                items = invoice.Items.Select(i => new
                {
                    itemId = i.ItemId,
                    name = i.Name,
                    price = i.Price,
                    quantity = i.Quantity
                })
            };

            return Ok(result);
        }

        /// <summary>
        /// Delete invoice
        /// </summary>
        /// <param name="id">Invoice ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<object>> DeleteInvoice(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);

            if (invoice == null)
            {
                return NotFound(new { message = $"Invoice with ID {id} not found" });
            }

            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Invoice with ID {id} deleted successfully" });
        }
    }

    public class CreateInvoiceRequest
    {
        public string CustomerName { get; set; } = string.Empty;
    }

    public class AddItemRequest
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
