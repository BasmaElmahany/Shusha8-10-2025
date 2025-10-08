using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shusha_project_BackUp
{
    public class Center
    {
        [Key]
        public int centerId { get; set; }

        public string centerName { get; set; }

        




    }
}
