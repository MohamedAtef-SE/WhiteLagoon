using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Application._Common.Utility;
using WhiteLagoon.Domain.Entities.Identity;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Controllers
{
    public class AccountController(UserManager<ApplicationUser> _userManager, SignInManager<ApplicationUser> _signInManager) : Controller
    {
        [HttpGet]
        public IActionResult Register(string? ReturnUrl = null)
        {
            ReturnUrl ??= Url.Content("~/");
            var registerVM = new RegisterVM()
            {
                ReturnUrl = ReturnUrl

            };
            return View(registerVM);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
                return View(registerVM);

            ApplicationUser newUser = new ApplicationUser()
            {
                CreatedAt = DateTime.UtcNow,
                Email = registerVM.Email,
                EmailConfirmed = true,
                Name = registerVM.Name,
                NormalizedEmail = registerVM.Email.ToUpper(),
                PhoneNumber = registerVM.PhoneNumber,
                UserName = registerVM.Email

            };

            var result = await _userManager.CreateAsync(newUser, registerVM.Password);
            if (result.Succeeded)
            {
                TempData["success"] = $"Welcome,{registerVM.Name}";

                if (!String.IsNullOrEmpty(registerVM.Role))
                {
                    await _userManager.AddToRoleAsync(newUser, registerVM.Role);
                }
                else
                {
                    await _userManager.AddToRoleAsync(newUser, SD.Role_Customer);
                }
                await _signInManager.SignInAsync(newUser, isPersistent: false);

                if (string.IsNullOrEmpty(registerVM.ReturnUrl))
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return LocalRedirect(registerVM.ReturnUrl);
                }
            }
            else
            {
                TempData["error"] = "Registeration failed";
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(registerVM);
            }

        }

        [HttpGet]
        public IActionResult Login(string? ReturnUrl = null)
        {
            ReturnUrl ??= Url.Content("~/");

            LoginViewModel loginViewModel = new LoginViewModel()
            {
                ReturnUrl = ReturnUrl
            };
            return View(loginViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
                return View(loginViewModel);
            var result = await _signInManager.PasswordSignInAsync(loginViewModel.Email,
                                                      loginViewModel.Password,
                                                      loginViewModel.RememberMe,
                                                      lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(loginViewModel.Email);
                var adminOnly = await _userManager.IsInRoleAsync(user!, SD.Role_Admin);
                if (adminOnly)
                {
                    return RedirectToAction("Index", "Dashboard");
                }


                TempData["success"] = $"Welcome, {loginViewModel.Email.Split('@')[0]}";
                if (string.IsNullOrEmpty(loginViewModel.ReturnUrl))
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return LocalRedirect(loginViewModel.ReturnUrl);
                }
            }
            TempData["error"] = "Invalid login";
            return View(loginViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
