using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvoiceApi.Models
{
    [Table("invoices")]
    public class Invoice
    {
        [Key]
        [Column("invoice_id")]
        public int InvoiceId { get; set; }

        [Column("customer_name")]
        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [Column("invoice_date")]
        public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;

        [Column("total_amount")]
        public decimal TotalAmount { get; set; }

        public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
    }
}
