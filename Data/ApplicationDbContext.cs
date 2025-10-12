using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shusha_project_BackUp.Data;
using Shusha_project_BackUp.Models;

namespace Shusha_project_BackUp
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet <Branch> Branches { get; set; }
        public DbSet<Center> Centers { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<Daily_Distribution> Daily_Distributions { get; set; }

        public DbSet<DailySales> Daily_Productions { get; set; }

        public DbSet<Eelectricity_Invoice> Electricity_Invoices { get; set; }

        public DbSet<Employees> Employees { get; set; }
        public DbSet<Feeds_distribution> Feeds_Distributions { get; set; }

        
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<sales> sales { get; set; }
        public DbSet<Solar_Invoices> Solar_Invoices { get; set; }
        public DbSet<Ward> Wards { get; set; }

        public DbSet<Data.DailySales> DailySales { get; set; }

        public DbSet<Medicine> medicines { get; set; }
        public DbSet<Medicine_Pruchase> Medicine_Pruchase { get; set; }

        public DbSet<MedicineUsed> medicineUsed  { get; set; }

       public DbSet<FeedInventory> feedInventories { get; set; }

        public DbSet<FeedPurchases> FeedPurchases { get; set; }

        public DbSet<Feed_Usage> feed_Usages { get; set; }

        public DbSet<Egg_stock> Egg_stock { get; set; }

        public DbSet<Prices> Prices { get; set; }

        public DbSet<Request_Proceeds > Request_Proceeds { get; set; }

        public DbSet<Proceed> Proceeds { get; set; }    

        public DbSet<WardsStock> wardsStocks { get; set; }

        public DbSet<Water_Invoices> Water_Invoices { get; set; } 


        public DbSet<totalProduction_vm> totalProduction_Vms { get; set; }

        public DbSet<TraderSales> TraderSales { get; set; }



        public DbSet<Total_Traders> Total_Traders { get; set; }

        public DbSet<HerdSales> HerdSales { get; set; }

        public DbSet<Waste_Sales> Waste_Sales { get; set; }











    }
}
