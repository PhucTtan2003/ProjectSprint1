using BacsiCreate.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BacsiCreate.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly HospitalDbContext _context;

        public DoctorsController(HospitalDbContext context)
        {
            _context = context;
        }

        // GET: Doctor/Index
        public IActionResult Index()
        {
            // Eager loading to include related Department data
            var doctors = _context.Doctors
                .Include(d => d.Department)
                .ToList();
            return View(doctors);
        }

        // GET: Doctor/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.DepartmentID = new SelectList(_context.Departments, "DepartmentID", "DepartmentName");
            ViewBag.DoctorID = new SelectList(_context.Doctors, "DoctorID", "FullName");
            return View();
        }

        // POST: Doctors/Create
        [HttpPost]
        public IActionResult Create(Doctor doctor)
        {
            try
            {
                _context.Doctors.Add(doctor);
                _context.SaveChanges();
            }
            catch (Exception ex) { return View(); }
            return RedirectToAction("Index");

        }
    }
}
