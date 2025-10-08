using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shusha_project_BackUp.Data
{
    public class WardsStock
    {
        [Key]
        public int stockID { get; set; }    

        public int whiteEggs { get; set; }
        public int rest_whEggs { get; set; }

        public int brownEggs { get; set; }
        public int rest_brEggs { get; set; }
        public decimal brokenEggs { get; set; }
        public decimal rest_bkEggs { get; set; }

        public decimal doubleEggs { get; set; }
        public decimal rest_dbEggs { get; set; }

        [ForeignKey("ward")]
        public int wardID { get; set; }

        public Ward? ward { get; set; }

        public DateOnly Date {  get; set; } 
    }
}
