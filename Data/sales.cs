using System.ComponentModel.DataAnnotations;

namespace Shusha_project_BackUp
{
    public class sales
    {
        [Key]
        public int sales_ID { get; set; }

        
        public int no_ofEggs { get; set; }

        public decimal Egg_Carton { get; set; }

        public double Hens_waste { get; set; }

        public decimal waste_price { get; set; }


        public int no_of_hens { get; set; }
        public decimal hen_price { get; set; }




    }
}
