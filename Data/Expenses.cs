using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Shusha_project_BackUp.Data
{

    public enum Expenses_type
    {
        [Display(Name = "مصروفات الاجور والمرتبات")]
        salaries = 1 ,
        [Display(Name = "مشتريات اعلاف")]
        feed = 2 ,
        [Display(Name = "مشتريات كتاكيت")]
        herd = 3 ,
        [Display(Name = "مشتريات ادوية ولقاحات ومبيدات")]
        medicine = 4 ,
        [Display(Name = "مشتريات اطباق الكرتون")]
        cartoon_plates =5 ,
        [Display(Name = "مشتريات مواد بترولية")]
        solar = 6,
        [Display(Name = "مصروفات تراخيص")]
        licence = 7 ,
        [Display(Name = "مصروفات تحليل عينات")]
        sample_analysis = 8 ,
        [Display(Name = "مصروفات مطبوعات")]
        prints = 9 ,
        [Display(Name = "مصروفات اعلانات")]
        adv = 10 ,
        [Display(Name = "مصروفات صيانة وقطع غيار")]
        maintainance = 11 ,
        [Display(Name = "كهرباء ومصروفات متنوعة")]
        electricity = 12 

    }


    public class Expenses
    {
        public int Id { get; set; }

        public Expenses_type type1 { get; set; }

        public decimal amount1 { get; set; }


        public DateOnly date {  get; set; } 
    }
}
