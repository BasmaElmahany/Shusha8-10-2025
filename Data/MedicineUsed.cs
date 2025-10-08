using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shusha_project_BackUp.Data
{
    public class MedicineUsed
    {
        [Key  ]
        public int id { get; set; } 

        [Required]
        public string name { get; set; }

        [Required]
        public string purpose { get; set; }

        [Required]
        public int quantityUsed { get; set; }

        [Required]
        public DateTime Date {  get; set; }
        [ForeignKey("Medicine")]
        [Required]
        public int MedicineID { get; set; } // Foreign key to Medicine


        public Medicine Medicine { get; set; }


        [ForeignKey("Ward")]
        public int WardID { get; set; }
        public Ward? Ward { get; set; }
    }
}
