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

        //بيض ابيض بشاير
        [Required, Range(0, int.MaxValue)]
        public int NoOfNewWhiteEggs { get; set; }

        [Required, Range(0, double.MaxValue)]
        public decimal NewWhiteEggPrice { get; set; }

        // بيض ابيض وسط
        [Required, Range(0, int.MaxValue)]
        public int NoOfMedWhiteEggs { get; set; }

        [Required, Range(0, double.MaxValue)]
        public decimal MedWhiteEggPrice { get; set; }


        //بيض بني عادي
        [Required, Range(0, int.MaxValue)]
        public int NoOfBrownEggs { get; set; }

        [Required, Range(0, double.MaxValue)]

        public decimal BrownEggPrice { get; set; }



        // بيض بني بشاير
        [Required, Range(0, int.MaxValue)]
        public int NoOfNewBrownEggs { get; set; }

        [Required, Range(0, double.MaxValue)]

        public decimal NewBrownEggPrice { get; set; }

        

        // بيض بني وسط

        [Required, Range(0, int.MaxValue)]
        public int NoOfMedBrownEggs { get; set; }

        [Required, Range(0, double.MaxValue)]

        public decimal BrownMedEggPrice { get; set; }


        [Required, Range(0, int.MaxValue)]
        public int NoOfBrokenEggs { get; set; }

        [Required, Range(0, double.MaxValue)]
        public decimal BrokenEggPrice { get; set; }


        // بيض دبل عادي
        [Required, Range(0, int.MaxValue)]
        public int NoOfDoubleEggs { get; set; }

        [Required, Range(0, double.MaxValue)]
        public decimal DoubleEggPrice { get; set; }



        // بيض دبل  بشاير
        [Required, Range(0, int.MaxValue)]
        public int NoOfNewDoubleEggs { get; set; }

        [Required, Range(0, double.MaxValue)]
        public decimal NewDoubleEggPrice { get; set; }


        //بيض دبل وسط
        [Required, Range(0, int.MaxValue)]
        public int NoOfMedDoubleEggs { get; set; }

        [Required, Range(0, double.MaxValue)]
        public decimal  MedDoubleEggPrice { get; set; }

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
                   (DoubleEggPrice * NoOfDoubleEggs) +
                   (NoOfNewWhiteEggs* NewWhiteEggPrice)+
                   (NoOfMedWhiteEggs* MedWhiteEggPrice)+
                   (NoOfNewBrownEggs* NewBrownEggPrice)+
                   (NoOfMedBrownEggs* BrownMedEggPrice) +
                   (NoOfNewDoubleEggs* NewDoubleEggPrice)+
                   (NoOfMedDoubleEggs* MedDoubleEggPrice);
        }

        // Method to update RequestProceed before saving
        public void UpdateRequestProceed()
        {
            RequestProceed = CalculateTotal();
        }
    }
}
