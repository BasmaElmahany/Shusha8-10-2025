using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shusha_project_BackUp
{
    public class Daily_Distribution
    {
        [Key]
        public int dis_id { get; set; }

        [ForeignKey("Center")]
        public int centerId { get; set; } 

        public Center? Center { get; set; }

       public int  WhEgg_platesnumber { get; set; }

        public int brEgg_platesnumber { get; set; }
        public decimal doubleEgg_platesnumber { get; set; }
        public decimal BrokenEgg_platesnumber { get; set; }


        public DateTime Date {  get; set; }
        [NotMapped]
        public int total=>(int)(WhEgg_platesnumber + brEgg_platesnumber + doubleEgg_platesnumber + BrokenEgg_platesnumber);


    }
}
