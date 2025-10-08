using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shusha_project_BackUp
{
    public class Eelectricity_Invoice
    {
        [Key]
        public int ID { get; set; }

        public DateTime Date { get; set; }


        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "القيمة يجب انت تكون اكبر من الصفر")]
        public decimal amount { get; set; }


        public string Invoice_photo { get; set; }


        [ForeignKey("branch")]
        public int BranchID { get; set; }
        public Branch? branch { get; set; }


    }
}
