using System.ComponentModel.DataAnnotations;

namespace Shusha_project_BackUp.Models
{
    public class totalProduction_vm
    {

        [Key]
        public int Id { get; set; }
        public int total_whiteEggs { get; set; }

        public int total_brownEggs { get; set; }

        public int total_brokenEggs { get; set; }

        public int total_doubleEggs { get; set; }



    }
}
