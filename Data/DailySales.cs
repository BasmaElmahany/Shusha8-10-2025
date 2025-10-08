using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shusha_project_BackUp.Data
{
    public class DailySales
    {
        [Key]
        public int dailySales_id { get; set; }

        [ForeignKey("ward")]
        public int wardID { get; set; }

        public Ward? ward { get; set; }

        public int no_of_carton_WhEggs { get; set; }

       
        public int no_of_carton_BrEggs { get; set; }
     
        public decimal no_of_carton_broken { get; set; }
       
        public decimal double_eggs { get; set; }
       

        public decimal? waste_poultry { get; set; }

      
        public DateOnly Date { get; set; }

        
       
    }
}
