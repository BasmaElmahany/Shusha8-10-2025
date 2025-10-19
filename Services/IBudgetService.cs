
using Microsoft.EntityFrameworkCore;
using Shusha_project_BackUp.Data;

namespace Shusha_project_BackUp.Services
{
    public interface IBudgetService
    {
        Task UpdateBudgetAsync(DateOnly date);
        Task RecalculateBudgetForYearAsync(int fiscalYear);
        int GetFiscalYear(DateOnly date);
        (DateOnly startDate, DateOnly endDate) GetFiscalYearRange(int fiscalYear);
        Task<BudgetComparison> GetBudgetComparisonAsync(int year1, int year2);
    }

    public class BudgetService : IBudgetService
    {
        private readonly ApplicationDbContext _context;

        public BudgetService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get fiscal year from a date (July 1 to June 30)
        /// Example: 2025-07-01 to 2026-06-30 = Fiscal Year 2025
        /// </summary>
        public int GetFiscalYear(DateOnly date)
        {
            if (date.Month >= 7) // July to December
            {
                return date.Year;
            }
            else // January to June
            {
                return date.Year - 1;
            }
        }

        /// <summary>
        /// Get the date range for a fiscal year
        /// </summary>
        public (DateOnly startDate, DateOnly endDate) GetFiscalYearRange(int fiscalYear)
        {
            var startDate = new DateOnly(fiscalYear, 7, 1);
            var endDate = new DateOnly(fiscalYear + 1, 6, 30);
            return (startDate, endDate);
        }

        /// <summary>
        /// Update budget when a Proceeds_Totals record is added/updated/deleted
        /// </summary>
        public async Task UpdateBudgetAsync(DateOnly date)
        {
            var fiscalYear = GetFiscalYear(date);
            await RecalculateBudgetForYearAsync(fiscalYear);
        }

        /// <summary>
        /// Recalculate budget for a specific fiscal year
        /// </summary>
        public async Task RecalculateBudgetForYearAsync(int fiscalYear)
        {
            var (startDate, endDate) = GetFiscalYearRange(fiscalYear);

            // Get all proceeds for this fiscal year
            var proceeds = await _context.proceeds_Totals
                .Where(p => p.Date >= startDate && p.Date <= endDate)
                .ToListAsync();

            // Calculate totals
            decimal totalEgg = proceeds.Sum(p => p.Egg + p.broken_Egg + p.double_Egg);
            decimal totalWaste = proceeds.Sum(p => p.Waste + p.waste_fees);
            decimal totalHerd = proceeds.Sum(p => p.herd);
            decimal totalMiscellaneous = proceeds.Sum(p => p.Miscellaneous);

            // Find or create budget for this year
            var currentBudget = await _context.Budget
                .FirstOrDefaultAsync(b => b.year == fiscalYear);

            if (currentBudget == null)
            {
                currentBudget = new Budget
                {
                    year = fiscalYear,
                    egg = totalEgg,
                    waste = totalWaste,
                    herd = totalHerd,
                    Miscellaneous = totalMiscellaneous
                };
                currentBudget.UpdateTotal();
                _context.Budget.Add(currentBudget);
            }
            else
            {
                currentBudget.egg = totalEgg;
                currentBudget.waste = totalWaste;
                currentBudget.herd = totalHerd;
                currentBudget.Miscellaneous = totalMiscellaneous;
                currentBudget.UpdateTotal();
                _context.Budget.Update(currentBudget);
            }

            await _context.SaveChangesAsync();

            // Update comparison with previous year
            await UpdateYearOverYearComparisonAsync(fiscalYear);
        }

        /// <summary>
        /// Update year-over-year comparison
        /// </summary>
        private async Task UpdateYearOverYearComparisonAsync(int fiscalYear)
        {
            var currentBudget = await _context.Budget
                .FirstOrDefaultAsync(b => b.year == fiscalYear);

            var previousBudget = await _context.Budget
                .FirstOrDefaultAsync(b => b.year == fiscalYear - 1);

            if (currentBudget != null && previousBudget != null)
            {
                // Calculate differences
                var eggDifference = currentBudget.egg - previousBudget.egg;
                var wasteDifference = currentBudget.waste - previousBudget.waste;
                var herdDifference = currentBudget.herd - previousBudget.herd;
                var miscellaneousDifference = currentBudget.Miscellaneous - previousBudget.Miscellaneous;
                var totalDifference = currentBudget.total - previousBudget.total;

                // Store comparison in a separate table if needed
                // Or you can add these fields to the Budget model
                // For now, we'll just log or use them in reports
            }
        }

        /// <summary>
        /// Get budget comparison between two years
        /// </summary>
        public async Task<BudgetComparison> GetBudgetComparisonAsync(int currentYear, int previousYear)
        {
            var currentBudget = await _context.Budget
                .FirstOrDefaultAsync(b => b.year == currentYear);

            var previousBudget = await _context.Budget
                .FirstOrDefaultAsync(b => b.year == previousYear);

            if (currentBudget == null || previousBudget == null)
            {
                return null;
            }

            return new BudgetComparison
            {
                CurrentYear = currentYear,
                PreviousYear = previousYear,
                CurrentBudget = currentBudget,
                PreviousBudget = previousBudget,
                EggDifference = currentBudget.egg - previousBudget.egg,
                WasteDifference = currentBudget.waste - previousBudget.waste,
                HerdDifference = currentBudget.herd - previousBudget.herd,
                MiscellaneousDifference = currentBudget.Miscellaneous - previousBudget.Miscellaneous,
                TotalDifference = currentBudget.total - previousBudget.total,
                EggPercentageChange = CalculatePercentageChange(previousBudget.egg, currentBudget.egg),
                WastePercentageChange = CalculatePercentageChange(previousBudget.waste, currentBudget.waste),
                HerdPercentageChange = CalculatePercentageChange(previousBudget.herd, currentBudget.herd),
                MiscellaneousPercentageChange = CalculatePercentageChange(previousBudget.Miscellaneous, currentBudget.Miscellaneous),
                TotalPercentageChange = CalculatePercentageChange(previousBudget.total, currentBudget.total)
            };
        }

        private decimal CalculatePercentageChange(decimal oldValue, decimal newValue)
        {
            if (oldValue == 0)
                return newValue > 0 ? 100 : 0;

            return ((newValue - oldValue) / oldValue) * 100;
        }
    }

    // Model for budget comparison
    public class BudgetComparison
    {
        public int CurrentYear { get; set; }
        public int PreviousYear { get; set; }
        public Budget CurrentBudget { get; set; }
        public Budget PreviousBudget { get; set; }

        // Differences
        public decimal EggDifference { get; set; }
        public decimal WasteDifference { get; set; }
        public decimal HerdDifference { get; set; }
        public decimal MiscellaneousDifference { get; set; }
        public decimal TotalDifference { get; set; }

        // Percentage changes
        public decimal EggPercentageChange { get; set; }
        public decimal WastePercentageChange { get; set; }
        public decimal HerdPercentageChange { get; set; }
        public decimal MiscellaneousPercentageChange { get; set; }
        public decimal TotalPercentageChange { get; set; }
    }
}

