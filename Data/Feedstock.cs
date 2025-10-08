using System.ComponentModel.DataAnnotations;

namespace Shusha_project_BackUp.Data
{
    public class Feedstock
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [Display(Name = "Feedstock Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Quantity (tons)")]
        public decimal Quantity { get; set; }

        [Required]
        [Display(Name = "Cost per Ton")]
        public decimal CostPerTon { get; set; }
    }
}
