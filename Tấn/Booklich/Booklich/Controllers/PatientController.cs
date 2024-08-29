using Booklich.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Booklich.Controllers
{
    public class PatientController : Controller
    {
        private readonly HospitalDbContext _context;

        public PatientController(HospitalDbContext context)
        {
            _context = context;
        }

        // GET: Patient/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Patient/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Patient patient)
        {
            if (ModelState.IsValid)
            {
                _context.Patients.Add(patient);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(patient);
        }

        // GET: Patient/Index
        public ActionResult Index()
        {
            var profiles = _context.Patients.ToList();
            return View(profiles);
        }

        // GET: Patient/Edit/5
        public ActionResult Edit(int id)
        {
            var patient = _context.Patients.Find(id);
            if (patient == null)
            {
                return NotFound();
            }
            return View(patient);
        }

        // POST: Patient/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Patient patient)
        {
            if (id != patient.PatientId) // Lưu ý: Sử dụng PatientID thay vì PatientId
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patient);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientExists(patient.PatientId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(patient);
        }

        // GET: Patient/Delete/5
        public ActionResult Delete(int id)
        {
            var patient = _context.Patients
                .FirstOrDefault(m => m.PatientId == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // POST: Patient/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var patient = _context.Patients.Find(id);
            _context.Patients.Remove(patient);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private bool PatientExists(int id)
        {
            return _context.Patients.Any(e => e.PatientId == id);
        }

        // POST: Patient/BookAppointment
        [HttpPost]
        public IActionResult BookAppointment(string specialty)
        {
            // Redirect to the SelectDoctor action with the chosen specialty
            return RedirectToAction("SelectDoctor", new { specialty });
        }

        // GET: Patient/SelectSpecialty
        public async Task<IActionResult> SelectSpecialty()
        {
            // Retrieve all specialties from the database
            var specialties = await _context.SelectSpecialties
                                            .Select(s => s.Specialty)
                                            .ToListAsync();

            if (specialties == null || specialties.Count == 0)
            {
                return RedirectToAction("Error", new { message = "Không tìm thấy chuyên khoa." });
            }

            return View(specialties);
        }

        // GET: Patient/SelectDoctor
        public async Task<IActionResult> SelectDoctor(string specialty)
        {
            // Retrieve all doctors with the selected specialty
            var doctors = await _context.Doctors
                                        .Where(d => d.Specialty == specialty)
                                        .ToListAsync();

            if (doctors.Count == 0)
            {
                return RedirectToAction("Error", new { message = "Không tìm thấy bác sĩ nào cho chuyên khoa này." });
            }

            ViewBag.Specialty = specialty;
            return View(doctors);
        }

        // GET: Patient/SelectTimeSlot
        public async Task<IActionResult> SelectTimeSlot(int doctorId, int patientId)
        {
            // Retrieve all available time slots for the selected doctor
            var timeSlots = await _context.TimeSlots
                                          .Where(t => t.DoctorId == doctorId && t.IsAvailable)
                                          .ToListAsync();

            if (timeSlots.Count == 0)
            {
                return RedirectToAction("Error", new { message = "Không có khung giờ nào có sẵn cho bác sĩ này." });
            }

            ViewBag.DoctorID = doctorId;
            ViewBag.PatientID = patientId;
            return View(timeSlots);
        }

        public async Task<IActionResult> ConfirmedAppointment(int? appointmentId)
        {
            // Kiểm tra xem appointmentId có giá trị hợp lệ không
            if (appointmentId == null || appointmentId <= 0)
            {
                // Nếu không hợp lệ, chuyển hướng tới trang lỗi hoặc trang chủ
                return RedirectToAction("Error", new { message = "Mã cuộc hẹn không hợp lệ." });
            }

            // Lấy thông tin cuộc hẹn theo ID
            var appointment = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);

            // Kiểm tra xem appointment có tồn tại không
            if (appointment == null)
            {
                // Nếu appointment không tồn tại, chuyển hướng tới trang lỗi
                return RedirectToAction("Error", new { message = "Không tìm thấy cuộc hẹn." });
            }

            // Truyền thông tin cuộc hẹn tới View
            return View(appointment);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmAppointment(int doctorId, int patientId, int timeSlotId)
        {
            // Tìm thông tin bác sĩ, bệnh nhân và khung giờ
            var doctor = await _context.Doctors.FindAsync(doctorId);
            var patient = await _context.Patients.FindAsync(patientId);
            var timeSlot = await _context.TimeSlots.FindAsync(timeSlotId);

            if (doctor == null || patient == null || timeSlot == null)
            {
                return RedirectToAction("Error", new { message = "Không tìm thấy thông tin bác sĩ, bệnh nhân, hoặc khung giờ." });
            }

            // Tạo cuộc hẹn mới
            var appointment = new Appointment
            {
                PatientId = patientId,
                DoctorId = doctorId,
                Fee = 150000m, // Chi phí có thể thay đổi
                Notes = "Khám dịch vụ" // Bạn có thể thay đổi ghi chú này tùy vào yêu cầu
            };

            // Lưu cuộc hẹn vào cơ sở dữ liệu
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            // Lấy mã cuộc hẹn sau khi lưu thành công
            int appointmentId = appointment.AppointmentId;

            // Chuyển hướng tới trang xác nhận với thông tin cuộc hẹn
            return RedirectToAction("ConfirmedAppointment", new { appointmentId });
        }
    }
}
