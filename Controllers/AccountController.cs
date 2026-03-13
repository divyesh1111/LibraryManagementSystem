using LibraryManagementSystem.Models;
using LibraryManagementSystem.ViewModels.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet, AllowAnonymous]
        public IActionResult Register(string? returnUrl = null) => View(new RegisterViewModel { ReturnUrl = returnUrl });

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);
            var user = new ApplicationUser { UserName = vm.Email, Email = vm.Email, FullName = vm.FullName, EmailConfirmed = true };
            var result = await _userManager.CreateAsync(user, vm.Password);
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors) ModelState.AddModelError("", e.Description);
                return View(vm);
            }
            await _userManager.AddToRoleAsync(user, "Member");
            await _signInManager.SignInAsync(user, isPersistent: false);
            return Redirect(vm.ReturnUrl ?? "/");
        }

        [HttpGet, AllowAnonymous]
        public IActionResult Login(string? returnUrl = null) => View(new LoginViewModel { ReturnUrl = returnUrl });

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);
            var result = await _signInManager.PasswordSignInAsync(vm.Email, vm.Password, vm.RememberMe, false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(vm);
            }
            return Redirect(vm.ReturnUrl ?? "/");
        }

        [HttpPost, Authorize, ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> Manage()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction(nameof(Login));
            return View(new ManageAccountViewModel { FullName = user.FullName ?? "", Email = user.Email ?? "" });
        }

        [HttpPost, Authorize, ValidateAntiForgeryToken]
        public async Task<IActionResult> Manage(ManageAccountViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction(nameof(Login));
            user.FullName = vm.FullName;
            await _userManager.UpdateAsync(user);
            TempData["SuccessMessage"] = "Profile updated.";
            return RedirectToAction(nameof(Manage));
        }

        [HttpGet]
        public IActionResult AccessDenied() => View();

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string? returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
        {
            returnUrl ??= "/";
            if (remoteError != null) return RedirectToAction(nameof(Login));
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null) return RedirectToAction(nameof(Login));

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
            if (result.Succeeded) return Redirect(returnUrl);

            var email = info.Principal.FindFirstValue(ClaimTypes.Email) ?? "";
            var name = info.Principal.FindFirstValue(ClaimTypes.Name) ?? "";
            return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = email, FullName = name, ReturnUrl = returnUrl });
        }

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null) return RedirectToAction(nameof(Login));

            var user = await _userManager.FindByEmailAsync(vm.Email);
            if (user == null)
            {
                user = new ApplicationUser { UserName = vm.Email, Email = vm.Email, FullName = vm.FullName, EmailConfirmed = true };
                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    foreach (var e in result.Errors) ModelState.AddModelError("", e.Description);
                    return View(vm);
                }
                await _userManager.AddToRoleAsync(user, "Member");
            }
            await _userManager.AddLoginAsync(user, info);
            await _signInManager.SignInAsync(user, false);
            return Redirect(vm.ReturnUrl ?? "/");
        }
    }
}