using InternProjectMVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Runtime.Serialization;

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

        [HttpPost]
        public async Task<IActionResult> Register(User model)
        {
            if (ModelState.IsValid)
            {
                //check user exist
                var user = await _Context.Users.FirstOrDefaultAsync(u => u.UserName == model.UserName);
                var email = await _Context.Users.FirstOrDefaultAsync(e => e.Email == model.Email);
                if(email != null)
                {
                    ModelState.AddModelError("Email", "Email already taken.");
                    return View(model);
                }
                if (user == null)
                {
                    // เข้ารหัสรหัสผ่าน
                    var hashedPassword = _PasswordHasher.HashPassword(model, model.Password);
                    model.Password = hashedPassword;
                    //save new user
                    _Context.Users.Add(model);
                    await _Context.SaveChangesAsync();
                    if(model.UserRole.ToString() == "Admin")
                    {
                        return RedirectToAction("Create","Employee");
                    }
                    else
                    {
                        return RedirectToAction("Index","Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("UserName", "Username already taken.");
                }

            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(User model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("Email", "Can't find the registered email.");
                var _loginhistory = new LogHistory
                {
                    Email = model.Email,
                    IpAddress = HttpContext.Connection.RemoteIpAddress.ToString(),
                    LoginDate = DateTime.Now,
                    LoginStatus = LogHistory.Status.fail
                };
                //เก็บข้อมูลลงloghistory table
                _Context.LogHistories.Add(_loginhistory);
                await _Context.SaveChangesAsync();
            }

            var user = await _Context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

                if (user != null)
                {
                    // ตรวจสอบรหัสผ่านที่กรอกกับรหัสผ่านที่เข้ารหัสแล้วในฐานข้อมูล
                    var result = _PasswordHasher.VerifyHashedPassword(user, user.Password, model.Password);
                    if (result == PasswordVerificationResult.Success)
                    {
                        var _loginhistory = new LogHistory
                        { 
                            Email = model.Email ,
                            IpAddress = HttpContext.Connection.RemoteIpAddress.ToString(),
                            LoginDate = DateTime.Now,
                            LoginStatus = LogHistory.Status.success 
                        };
                        //เก็บข้อมูลลงloghistory table
                        _Context.LogHistories.Add(_loginhistory);
                        await _Context.SaveChangesAsync();

                    if (user.UserRole == Models.User.Role.Admin)
                    {
                        // สมมติว่าการ Login สำเร็จ
                        HttpContext.Session.SetString("Username", user.UserName);
                        return RedirectToAction("Create", "Employee");
                    }
                    else
                    {
                        // สมมติว่าการ Login สำเร็จ
                        HttpContext.Session.SetString("Username", user.UserName);
                        return RedirectToAction("Index", "Home");
                    }

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