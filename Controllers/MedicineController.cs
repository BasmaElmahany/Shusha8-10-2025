using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shusha_project_BackUp.Data;
using Shusha_project_BackUp.Data.Migrations;
using Shusha_project_BackUp.Models;
using System;
using System.Linq;

namespace Shusha_project_BackUp.Controllers
{
    [Authorize(Roles = "Admin,Accountant")]
    public class MedicineController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MedicineController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Display available medicines
        public IActionResult Index()
        {
            var medicines = _context.medicines.ToList();
            return View(medicines);
        }

        // Display the medicine details and form to buy
        public IActionResult Buy(int id)
        {
            var medicine = _context.medicines.FirstOrDefault(m => m.ID == id);
            if (medicine == null)
            {
                return NotFound();
            }

            var viewModel = new BuyMedicineViewModel
            {
                MedicineID = medicine.ID,
                MedicineName = medicine.Name,
                MedicinePrice = medicine.Price
            };

            return View(viewModel);
        }

        public IActionResult use(int id)
        {
            var medicine = _context.medicines.FirstOrDefault(m => m.ID == id);
            if (medicine == null)
            {
                return NotFound();
            }

            // Preparing view model
            var viewModel = new MedicineUsage
            {
                medicineID = medicine.ID,
                medicineName = medicine.Name
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult use(MedicineUsage model)
        {
            if (ModelState.IsValid)
            {
                // Fetch the medicine from the database
                var medicine = _context.medicines.FirstOrDefault(m => m.ID == model.medicineID);
                if (medicine != null)
                {
                    // Check if the used quantity is greater than the available quantity
                    if (model.use_qty > medicine.Quantity)
                    {
                        // Return error if not enough stock is available
                        ModelState.AddModelError("use_qty", "الكمية المطلوبة أكبر من الكمية المتوفرة.");
                        return View(model);
                    }

                    // Update the medicine's quantity after usage
                    medicine.Quantity -= model.use_qty;

                    // Create a new usage record
                    var usage = new MedicineUsed
                    {
                        MedicineID = model.medicineID,
                        quantityUsed = model.use_qty,
                        Date = DateTime.Now,
                        purpose = model.Purpose,
                        name = model.medicineName // This is now the purpose provided by the user
                    };

                    // Save the usage record to the database
                    _context.medicineUsed.Add(usage);

                    // Save the changes to the database
                    _context.SaveChanges();

                    // Redirect to the index page or another appropriate page
                    return RedirectToAction("Index");
                }
                else
                {
                    // If medicine is not found, return NotFound
                    return NotFound();
                }
            }

            // If model is not valid, return to the same page with error messages
            return View(model);
        }



        // Handle the purchase action and update the quantity
        [HttpPost]
        public IActionResult Buy(BuyMedicineViewModel model)
        {
            if (ModelState.IsValid)
            {
                var medicine = _context.medicines.FirstOrDefault(m => m.ID == model.MedicineID);
                if (medicine != null)
                {
                    

                    // Update the medicine's quantity after purchase
                    medicine.Quantity += model.QuantityToBuy;
                    var TotalPrice= model.QuantityToBuy * medicine.Price;
                    // Create a new purchase record
                    var purchase = new Medicine_Pruchase
                    {
                        MedicineID = model.MedicineID,
                        Quantity = model.QuantityToBuy,
                        PurchaseDate = DateTime.Now,
                        Total = TotalPrice,
                        MedicineName= medicine.Name,
                        Supplier = model.Supplier
                    };

                    // Save the purchase record to the database
                    _context.Medicine_Pruchase.Add(purchase);

                    // Save the changes to the database
                    _context.SaveChanges();

                    // Redirect to the medicine list or a confirmation page
                    return RedirectToAction("Index");
                }
                else
                {
                    // Handle medicine not found
                    return NotFound();
                }
            }

            // If model is not valid, return to the same page with error messages
            return View(model);
        }


        // Create GET and POST methods for Medicine
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Medicine medicine)
        {
            if (ModelState.IsValid)
            {
                _context.medicines.Add(medicine);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));  // Redirect to the index view after saving
            }
            return View(medicine); // Return the view with error messages if validation fails
        }
        // Edit GET method for fetching the medicine by ID
        public IActionResult Edit(int id)
        {
            var medicine = _context.medicines.FirstOrDefault(m => m.ID == id);
            if (medicine == null)
            {
                return NotFound();
            }
            return View(medicine);
        }

        // Edit POST method for saving the edited medicine
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Medicine medicine)
        {
            if (id != medicine.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(medicine);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(medicine);
        }

        // Delete GET method to confirm the deletion
        public IActionResult Delete(int id)
        {
            var medicine = _context.medicines.FirstOrDefault(m => m.ID == id);
            if (medicine == null)
            {
                return NotFound();
            }
            return View(medicine);
        }

        // Delete POST method to delete the record
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var medicine = _context.medicines.FirstOrDefault(m => m.ID == id);
            if (medicine == null)
            {
                return NotFound();
            }

            _context.medicines.Remove(medicine);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        // Details method to show the medicine details
        public IActionResult Details(int id)
        {
            var medicine = _context.medicines.FirstOrDefault(m => m.ID == id);
            if (medicine == null)
            {
                return NotFound();
            }
            return View(medicine);
        }

    }

}
