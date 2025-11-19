using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvoiceApi.Models
{
    [Table("invoice_items")]
    public class InvoiceItem
    {
        [Key]
        [Column("item_id")]
        public int ItemId { get; set; }

        [Column("invoice_id")]
        [ForeignKey("Invoice")]
        public int InvoiceId { get; set; }

        [Column("name")]
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Column("price")]
        public decimal Price { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; } = 1;

        public Invoice? Invoice { get; set; }
    }
}
