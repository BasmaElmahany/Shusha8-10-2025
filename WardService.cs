using Hangfire;
using Microsoft.EntityFrameworkCore;
namespace Shusha_project_BackUp
{
    internal class WardService
    {
        private readonly ApplicationDbContext _context;
        public WardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void IncrementWardAges()
        {
            var wards = _context.Wards.ToList();
            foreach (var ward in wards)
            {
                ward.WhAge += 1;
                ward.BrAge += 1;
            }

            _context.SaveChanges();
        }
    }
}