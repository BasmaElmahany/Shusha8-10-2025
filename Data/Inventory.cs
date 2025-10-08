using System.ComponentModel.DataAnnotations;

namespace Shusha_project_BackUp
{
    public class Inventory
    {
        [Key]
        public int Inv_Id { get; set;} 

        public int No_of_Wheggs {  get; set;}

        public int No_of_Breggs { get; set; }

        public int No_of_Brokeneggs { get; set; } 

        public int No_of_Double_eggs { get; set; }

        public double Feeds {  get; set; } 

        public double Hens_waste { get; set;} 

        public DateOnly Date { get; set; } 

        
         
    }
}
