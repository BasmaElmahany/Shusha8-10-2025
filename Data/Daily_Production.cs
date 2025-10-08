using Shusha_project_BackUp.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shusha_project_BackUp
{
    public class DailySales
    {
        [Key]
        public int id { get; set; }

        public  Ward? ward{ get; set; }

        [ForeignKey("ward")]
        public int? ward_id { get; set; }

        public decimal No_of_Wheggs { get; set; }
        public decimal No_of_Breggs { get; set; }
        public int Whdead_herd { get; set; }
        public int Brdead_herd { get; set; }
        public decimal double_eggs { get; set; }

        public decimal no_of_carton_broken { get; set; }
        public DateOnly Date { get; set; }
        // New Feed Production Property
      




    }
}
