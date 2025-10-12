using System.ComponentModel.DataAnnotations;

namespace Shusha_project_BackUp.Data
{
    public class Waste_Sales
    {
        [Key]
        public int Id { get; set; }

        [Required, Range(0, double.MaxValue)]
        public decimal wasteMeters { get; set; }


        [Required, Range(0, double.MaxValue)]
        public decimal price { get; set; }

        public string? TraderName { get; set; } 

        public DateOnly date {  get; set; }


    }
}
