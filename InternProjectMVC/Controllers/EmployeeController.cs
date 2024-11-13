using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InternProjectMVC.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly MyDatabaseContext _context;

        public EmployeeController(MyDatabaseContext context)
        {
            _context = context;  // ต้องแน่ใจว่า context ไม่เป็น null
        }

        //GET : Employee/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            // ตรวจสอบว่าอีเมลนี้มีอยู่ในฐานข้อมูลแล้วหรือยัง
            var existingEmail = await _context.Employees
                .FirstOrDefaultAsync(e => e.Email == employee.Email);

            if (existingEmail != null)
            {
                // ถ้ามีข้อมูลซ้ำ ให้แสดงข้อความหรือแจ้งเตือนผู้ใช้
                ModelState.AddModelError("Email", "The email is already in use.");
                return View(employee);  // ส่งข้อมูลกลับไปที่ View
            }


            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details","Employee");  // หรือหน้าที่จะไปหลังจากบันทึก
            }
            return View(employee);
        }

        //GET : Employee/Details
        public async Task<IActionResult> Details()
        {
            // ดึงข้อมูลทั้งหมดจากตาราง Employee
            var employees = await _context.Employees.ToListAsync();

            // ส่งข้อมูลไปยัง View
            return View(employees);
        }

        //ส่งหน้า edit ไปให้ user
        // GET: Employee/Edit/{id}
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) 
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var employee = await _context.Employees.FindAsync(id);


                if (employee == null)
                {
                    return NotFound();
                }
                return View(employee);  // ส่งข้อมูล Employee ไปยัง View
            }
            return View();
            
        }


        //รับdataจากuserมาedit DB
        // POST: Employee/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,Employee employee)
        {
            // ค้นหาผู้ใช้ปัจจุบันตาม ID
            var existingUser = await _context.Employees.FindAsync(id);

            if (existingUser == null)
            {
                return NotFound(); // หากไม่พบผู้ใช้ให้ส่งกลับ 404
            }

            // ตรวจสอบว่ามี Email ซ้ำกันในระบบหรือไม่ โดยยกเว้น ID ของผู้ใช้งานปัจจุบัน
            bool isEmailDuplicate = await _context.Employees
                .AnyAsync(u => u.Email == employee.Email && u.ID != id);

            if (isEmailDuplicate)
            {
                // ถ้ามี Email ซ้ำ แสดงข้อผิดพลาดใน ModelState
                ModelState.AddModelError("Email", "The email is already in use.");
                return View(employee); // กลับไปยัง View พร้อมแสดงข้อผิดพลาด
            }

            if (ModelState.IsValid)
            {

                // หากไม่มี Email ซ้ำให้ทำการอัปเดตข้อมูล
                existingUser.FirstName = employee.FirstName;
                existingUser.LastName = employee.LastName;
                existingUser.BirthDate = employee.BirthDate;
                existingUser.Email = employee.Email;
                existingUser.PhoneNumber = employee.PhoneNumber;
                existingUser.JobTitle = employee.JobTitle;

                // บันทึกการเปลี่ยนแปลง
                await _context.SaveChangesAsync();

                return RedirectToAction("Details","Employee");  // หลังจากอัปเดตแล้ว redirect ไปที่หน้า Index
            }
            return View(employee);  // ถ้ามีข้อผิดพลาดใน ModelState จะกลับไปที่ฟอร์ม
        }

        public IActionResult Search(string? searchTerm, DateTime? birthDate, string? jobTitle)
        {
            // เริ่มต้นตัวแปร employees
            var employees = _context.Employees.AsQueryable();

            if (ModelState.IsValid)
            {
                // ค้นหาตามชื่อหรือนามสกุล
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    string[] words = searchTerm.Split(' ');
                    if (words.Length > 1)
                    {
                        employees = employees.Where(e => e.FirstName.Contains(searchTerm) || e.LastName.Contains(searchTerm) || e.FirstName.Contains(words[0]) || e.FirstName.Contains(words[1]));
                    }
                    else
                    {
                        employees = employees.Where(e => e.FirstName.Contains(searchTerm) || e.LastName.Contains(searchTerm) || e.FirstName.Contains(words[0]));
                    }

                }

                // ค้นหาตามวันเกิด
                if (birthDate.HasValue)
                {
                    employees = employees.Where(e => e.BirthDate.Date == birthDate.Value.Date);
                }

                // ค้นหาตามตำแหน่งงาน
                if (!string.IsNullOrEmpty(jobTitle))
                {
                    employees = employees.Where(e => e.JobTitle.Contains(jobTitle));
                }
            }
            // ส่งผลลัพธ์ไปที่ View
            return View(employees.ToList());
        }

        // แสดงหน้า Confirm Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var employee = await _context.Employees.FindAsync(id);
                if (employee == null)
                {
                    return NotFound();
                }
                return View(employee);
            }
            return View();
        }

        // ทำการลบข้อมูลในฐานข้อมูล
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (ModelState.IsValid)
            {
                var employee = await _context.Employees.FindAsync(id);
                if (employee != null)
                {
                    _context.Employees.Remove(employee);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("Details", "Employee"); // กลับไปยังหน้ารายการหลักหลังจากลบสำเร็จ
            }
            return View();
        }
    }
}
