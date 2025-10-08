using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Shusha_project_BackUp.Data
{
    public class TraderSales
    {
        [Key]
        public int Id { get; set; }

        [Required, Range(0, int.MaxValue)]
        public int NoOfWhiteEggs { get; set; }

        [Required, Range(0, double.MaxValue)]
        public decimal WhiteEggPrice { get; set; }

        [Required, Range(0, int.MaxValue)]
        public int NoOfBrownEggs { get; set; }

        [Required, Range(0, double.MaxValue)]
        public decimal BrownEggPrice { get; set; }

        [Required, Range(0, int.MaxValue)]
        public int NoOfBrokenEggs { get; set; }

        [Required, Range(0, double.MaxValue)]
        public decimal BrokenEggPrice { get; set; }

        [Required, Range(0, int.MaxValue)]
        public int NoOfDoubleEggs { get; set; }

        [Required, Range(0, double.MaxValue)]
        public decimal DoubleEggPrice { get; set; }

        public bool IsPaid { get; set; }

        public DateOnly date { get; set; }

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal RequestProceed { get; set; } // Now mapped to the database

        public string name_ofTrader { get; set; }

        // Method to calculate total revenue
        public decimal CalculateTotal()
        {
            return (WhiteEggPrice * NoOfWhiteEggs) +
                   (BrownEggPrice * NoOfBrownEggs) +
                   (BrokenEggPrice * NoOfBrokenEggs) +
                   (DoubleEggPrice * NoOfDoubleEggs);
        }

        // Method to update RequestProceed before saving
        public void UpdateRequestProceed()
        {
            RequestProceed = CalculateTotal();
        }
    }
}
