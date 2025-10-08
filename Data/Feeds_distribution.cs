using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shusha_project_BackUp
{
    public class Feeds_distribution
    {
        [Key]
        public int dis_Id { get; set; }

        public decimal quantity { get; set; }


        public Ward? ward { get; set; }

        [ForeignKey("ward")]
        public int WardID { get; set; }

        public DateOnly Date { get; set; }
    }
}
