using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shusha_project_BackUp
{
    public class Ward
    {
        [Key]
        public  int Ward_ID { get; set; }
        [Required(ErrorMessage = "يرجى إدخال اسم العنبر")]
        public string WardName { get; set; }
        [Required(ErrorMessage = "يرجى إدخال عدد القطيع الأبيض")]
        public int No_of_Whherd { get; set; }

        public int WhAge { get; set; }

        [Required(ErrorMessage = "يرجى إدخال عدد القطيع البني")]
        public int No_of_Brherd { get; set; }

        public int BrAge { get; set; }  

        public virtual Branch? branch { get; set; }
        [Required(ErrorMessage = "يرجى اختيار المشروع")]
        [ForeignKey("branch")]
        public int branchId { get; set; }

        public DateOnly Date { get; set; }
    }
}
