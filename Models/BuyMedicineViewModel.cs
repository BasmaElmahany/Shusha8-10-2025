using System.ComponentModel.DataAnnotations;
namespace Shusha_project_BackUp.Models
{
    public class BuyMedicineViewModel
    {
        public int MedicineID { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "الكمية يجب أن تكون أكبر من 0")]
        public int QuantityToBuy { get; set; }

        public string MedicineName { get; set; }
        public decimal MedicinePrice { get; set; }

        public string Supplier { get; set; }
    }
}

