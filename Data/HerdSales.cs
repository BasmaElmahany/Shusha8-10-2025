using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace Shusha_project_BackUp.Data
{
    public class HerdSales
    {
        [Key]
        public int id { get; set; }

        // عدد القطيع الابيض
        public int NO_wh_herd { get; set; } 
        // وزن كل  دجاجة من القطيع الابيض بالكيلو
        public double weight_white { get; set; }

        // سعر الكيلو للابيض
        public double wh_price_kilo { get; set; }
        // اجمالي المطلوب
        public double req_total_wh {  get; set; }

        // عدد القطيع البني
        public int NO_br_herd { get; set; }

        // وزن كل  دجاجة من القطيع البني بالكيلو
        public double weight_brown { get; set; }

        // سعر الكيلو للبني
        public double br_price_kilo { get; set; }
        // اجمالي المطلوب
        public double req_total_br { get; set; }

        // التاريخ 
        public DateOnly Date {  get; set; }
        // اجمالي المطلوب
        public double total_request_proceed { get; set; }
        // هل تم التسديد ؟
         public bool  IsPaid {  get; set; }
    }
}
