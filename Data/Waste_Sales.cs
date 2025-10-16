using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shusha_project_BackUp.Data
{
    public class Waste_Sales
    {
        [Key]
        public int Id { get; set; }

        [Required, Range(0, double.MaxValue)]
        public decimal wasteMeters { get; set; }
        public decimal wasteFees { get; set; }

        public Branch? branch { get; set; }

        [ForeignKey("branch")]
        public int? branchId { get; set; }

        [Required, Range(0, double.MaxValue)]
        public decimal price { get; set; }

        public string? TraderName { get; set; } 

        public DateOnly date {  get; set; }


    }
}
