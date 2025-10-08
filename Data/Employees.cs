using System.ComponentModel.DataAnnotations;

namespace Shusha_project_BackUp
{
    public class Employees
    {
        [Key]
        public int emp_id { get; set; }

        [Required(ErrorMessage = "اسم الموظف مطلوب")]
        public string emp_name { get; set; }
        [Required(ErrorMessage = "الرقم القومي مطلوب")]
        [StringLength(14, MinimumLength = 14, ErrorMessage = "الرقم القومي يجب أن يتكون من 14 رقمًا")]
        public string NID { get; set;}
        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [Phone(ErrorMessage = "رقم الهاتف غير صحيح")]
        public string phoneNumber { get; set; }

        public Contract? contract { get; set; }




    }
}
