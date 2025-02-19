
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BookingRIo.Areas.Admin.ViewModels;
using BookingRIo.DTO;
using BookingRIo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BookingRIo.Controllers;

public class AuthController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IHttpContextAccessor _httpContext;
    private readonly ILogger<AuthController> _logger;


    public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, IHttpContextAccessor httpContext, RoleManager<IdentityRole> roleManager, ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _httpContext = httpContext;
        _roleManager = roleManager;
        _logger = logger;
    }
    public IActionResult Login()
    {
        var user = _httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (user != null)
        {
            return RedirectToAction("Index", "Home");
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {


        var findUser = await _userManager.FindByEmailAsync(loginDto.Email);
        if (findUser == null)
            return View();


        var result = await _signInManager.PasswordSignInAsync(findUser, loginDto.Password, true, true);
        if (result.Succeeded)
            return RedirectToAction("Index", "Home");

        return View();
    }
    //GET, POST
    //AOP

    [HttpGet] // Attribute
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        var findUser = await _userManager.FindByEmailAsync(registerDto.Email);

        if (findUser != null)
            return View();

        User newUser = new()
        {
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            Email = registerDto.Email,
            UserName = registerDto.Email,
           // PhotoUrl = "/",
          //  About = " ",
            ConfirmationToken = Guid.NewGuid().ToString()
        };

        IdentityResult result = await _userManager.CreateAsync(newUser, registerDto.Password);


        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(newUser, "User");
            return RedirectToAction("Login");
        }


        return View();
    }

    public IActionResult Forgot()
    {
        return View();
    }


    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordVIewModel model)
    {
        if (ModelState.IsValid)
        {
            // Find the user by email
            var user = await _userManager.FindByEmailAsync(model.Email);
            // If the user is found AND Email is confirmed
            if (user.Email != null && await _userManager.IsEmailConfirmedAsync(user))
            {
                // Generate the reset password token
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                // Build the password reset link
                var passwordResetLink = Url.Action("ResetPassword", "Account",
                        new { email = model.Email, token = token }, Request.Scheme);

                // Log the password reset link
                _logger.Log(LogLevel.Warning, passwordResetLink);

                // Send the user to Forgot Password Confirmation view
                TempData["Feedback"] = "Please check your email";
                return View("ForgotPasswordConfirmation");
            }
            else
            {
                TempData["Feedback"] = "This email cannot exist";
            }
            // To avoid account enumeration and brute force attacks, don't
            // reveal that the user does not exist or is not confirmed
            return View("ForgotPasswordConfirmation");
        }

        return View(model);
    }

    public IActionResult Logout()
    {

        _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }
}
