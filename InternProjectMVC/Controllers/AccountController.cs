using InternProjectMVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Runtime.Serialization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace InternProjectMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly MyDatabaseContext _Context;
        private readonly PasswordHasher<User> _PasswordHasher;

        public AccountController(MyDatabaseContext context)
        {
            _Context = context;
            _PasswordHasher = new PasswordHasher<User>();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Authentication()
        {
            return View();
        }

        public async Task<IActionResult> Verification()
        {
            // ดึงข้อมูลจาก table PendingAuth
            var PendingUser = await _Context.PendingAuths.ToListAsync();
            return View(PendingUser);
        }

        // Verifier User
        public async Task<IActionResult> VerifierUser(int? id, string status)
        {
            // หา pending user ผ่าน id
            var _user = await _Context.PendingAuths.FindAsync(id);
            if (_user != null)
            {
                var existUser = await _Context.Certifieds.FirstOrDefaultAsync(c => c.UserName == _user.UserName);

                // ยังไม่ได้ให้สิทธิ์
                if (existUser == null)
                {
                    // status check
                    if (status.Equals("pass"))
                    {
                        // create data
                        var _User = new Certified
                        {
                            UserName = _user.UserName,
                            AuthenBy = "Admin",
                            Status = status
                        };

                        // add data to Certifieds table
                        await _Context.Certifieds.AddAsync(_User);
                        await _Context.SaveChangesAsync();

                        // ลบ data ออกจาก pending table
                        var _exisUser = await _Context.PendingAuths.FindAsync(_user.Id);
                        if (_exisUser != null)
                        {
                            _Context.PendingAuths.Remove(_exisUser);
                            await _Context.SaveChangesAsync();
                            return RedirectToAction("Verification", "Account");
                        }

                        return RedirectToAction("Create", "Employee");
                    }

                    if (status.Equals("reject")) // status reject
                    {
                        // create data
                        var _User = new Certified
                        {
                            UserName = _user.UserName,
                            AuthenBy = "Admin",
                            Status = status
                        };

                        // add data to Certifieds table
                        await _Context.Certifieds.AddAsync(_User);
                        await _Context.SaveChangesAsync();

                        // ลบ data ออกจาก pending table
                        var _exisUser = await _Context.PendingAuths.FindAsync(_user.Id);
                        if (_exisUser != null)
                        {
                            _Context.PendingAuths.Remove(_exisUser);
                            await _Context.SaveChangesAsync();
                            return RedirectToAction("Verification", "Account");
                        }

                        return RedirectToAction("Verification", "Account");
                    }
                }
                else
                {
                    // status check
                    if (status.Equals("pass"))
                    {
                        // find ข้อมูลที่ต้อง update
                        var findUser = await _Context.Certifieds.FirstOrDefaultAsync(f => f.UserName == existUser.UserName);
                        if (findUser != null)
                        {
                            findUser.Status = "pass";
                            _Context.Certifieds.Update(findUser);
                            await _Context.SaveChangesAsync();
                            // ลบ data ออกจาก pending table
                            var _exisUser = await _Context.PendingAuths.FindAsync(_user.Id);
                            if (_exisUser != null)
                            {
                                _Context.PendingAuths.Remove(_exisUser);
                                await _Context.SaveChangesAsync();
                                return RedirectToAction("Verification", "Account");
                            }
                            return RedirectToAction("Verification", "Account");
                        }
                    }

                    // status check
                    if (status.Equals("reject"))
                    {
                        // find ข้อมูลที่ต้อง update
                        var findUser = await _Context.Certifieds.FirstOrDefaultAsync(f => f.UserName == existUser.UserName);
                        if (findUser != null)
                        {
                            findUser.Status = "reject";
                            _Context.Certifieds.Update(findUser);
                            await _Context.SaveChangesAsync();
                            // ลบ data ออกจาก pending table
                            var _exisUser = await _Context.PendingAuths.FindAsync(_user.Id);
                            if (_exisUser != null)
                            {
                                _Context.PendingAuths.Remove(_exisUser);
                                await _Context.SaveChangesAsync();
                                return RedirectToAction("Verification", "Account");
                            }
                            return RedirectToAction("Verification", "Account");
                        }
                    }
                }
                return RedirectToAction("Verification", "Account");
            }
            return RedirectToAction("Verification", "Account");
        }

        [HttpPost]
        public async Task<IActionResult> Register(User model)
        {
            if (ModelState.IsValid)
            {
                // check user exist
                var user = await _Context.Users.FirstOrDefaultAsync(u => u.UserName == model.UserName);
                var email = await _Context.Users.FirstOrDefaultAsync(e => e.Email == model.Email);

                if (email != null)
                {
                    ModelState.AddModelError("Email", "Email already taken.");
                    return View(model);
                }

                if (user == null)
                {
                    // เข้ารหัสรหัสผ่าน
                    var hashedPassword = _PasswordHasher.HashPassword(model, model.Password);
                    model.Password = hashedPassword;

                    // save new user
                    _Context.Users.Add(model);
                    await _Context.SaveChangesAsync();

                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    ModelState.AddModelError("UserName", "Username already taken.");
                    return View(model);
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(User model)
        {
            // check validate form
            if (!ModelState.IsValid)
            {
                var user = await _Context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

                // เมื่อค้นเจอ email
                if (user != null)
                {
                    // ตรวจสอบรหัสผ่านที่กรอกกับรหัสผ่านที่เข้ารหัสแล้วในฐานข้อมูล
                    var result = _PasswordHasher.VerifyHashedPassword(user, user.Password, model.Password);
                    if (result == PasswordVerificationResult.Success)
                    {
                        string ipv6 = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "None IP";
                        var _loginhistory = new LogHistory
                        {
                            Email = model.Email,
                            IpAddress = ipv6,
                            LoginDate = DateTime.Now,
                            LoginStatus = LogHistory.Status.success
                        };

                        // เก็บข้อมูลลง loghistory table
                        _Context.LogHistories.Add(_loginhistory);
                        await _Context.SaveChangesAsync();

                        // หาคนที่ username ตรงกับที่ user กรอกเข้ามา
                        var IsValidUser = await _Context.Certifieds.FirstOrDefaultAsync(c => c.UserName == user.UserName && c.Status == "pass");

                        if (IsValidUser != null)
                        {
                            // สมมติว่าการ Login สำเร็จ
                            HttpContext.Session.SetString("Username", user.UserName);
                            return RedirectToAction("Create", "Employee");
                        }
                        else
                        {
                            // เพิ่ม data ใน pending table
                            var USER = new PendingAuth
                            {
                                UserName = user.UserName,
                                UserRole = user.UserRole
                            };
                            await _Context.PendingAuths.AddAsync(USER);
                            await _Context.SaveChangesAsync();

                            // สมมติว่าการ Login ไม่สำเร็จ
                            HttpContext.Session.SetString("Username", user.UserName);
                            return RedirectToAction("Authentication", "Account");
                        }
                    }
                    else // password incorrect
                    {
                        ModelState.AddModelError("Password", "Password Incorrect");
                        return View(model);
                    }
                }
                else
                {
                    string ipv6 = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "None IP";
                    var _loginhistory = new LogHistory
                    {
                        Email = model.Email,
                        IpAddress = ipv6,
                        LoginDate = DateTime.Now,
                        LoginStatus = LogHistory.Status.fail
                    };

                    // เก็บข้อมูลลง loghistory table
                    _Context.LogHistories.Add(_loginhistory);
                    await _Context.SaveChangesAsync();
                }
            }

            return View(model);
        }

        public IActionResult Logout()
        {
            // ล้าง Session หรือ Cookies ที่ใช้เก็บสถานะการเข้าสู่ระบบ
            HttpContext.Session.Clear();

            return RedirectToAction("Login", "Account"); // เปลี่ยนเส้นทางไปหน้า Login หรือหน้าแรก
        }
    }
}
