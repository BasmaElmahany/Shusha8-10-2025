using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shusha_project_BackUp.Data
{
    public class Medicine_Pruchase
    {
        [Key]
        public int ID { get; set; }
        [ForeignKey("Medicine")]
        [Required]
        public int MedicineID { get; set; } // Foreign key to Medicine

        [Required]
        public int Quantity { get; set; }
        
        public string MedicineName { get; set; }
        [Required]
        public DateTime PurchaseDate { get; set; }

        public Medicine Medicine { get; set; }

        public string Supplier { get; set; }
        public decimal Total { get; set; } 


    }
}
