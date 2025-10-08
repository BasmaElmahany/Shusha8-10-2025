using System.ComponentModel.DataAnnotations;

namespace Shusha_project_BackUp
{
    public class Branch
    {
        [Key]
        public int branch_id { get; set; }
        public string branch_name { get; set; }

        public ICollection<Ward>? wards { get; set; }
        

        public int total_no_of_Whherd
        {
            get
            {
                return wards != null ? wards.Sum(w => w.No_of_Whherd) : 0;
            }
        }
        public int total_no_of_Brherd
        {
            get
            {
                return wards != null ? wards.Sum(w => w.No_of_Brherd) : 0;
            }
        }
    }
}
