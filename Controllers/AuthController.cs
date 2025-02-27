
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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;



namespace BookingRIo.Controllers;

public class AuthController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IHttpContextAccessor _httpContext;
    private readonly ILogger<AuthController> _logger;
    private readonly EmailSender _emailSender;
    private readonly IConfiguration _configuration;


    public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, IHttpContextAccessor httpContext, RoleManager<IdentityRole> roleManager, ILogger<AuthController> logger, EmailSender emailSender, IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _httpContext = httpContext;
        _roleManager = roleManager;
        _logger = logger;
        _emailSender = emailSender;
        _configuration = configuration;
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

    [HttpGet] 
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

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Forgot()
    {
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> ForgotPassword(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return View("ForgotPasswordConfirmation");
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetLink = Url.Action("ResetPassword", "Account", new { token, email = user.Email }, Request.Scheme);

        await _emailSender.SendEmailAsync(user.Email, "Reset Your Password", resetLink);

        return View("ForgotPasswordConfirmation");
    }


    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResetPassword(string token, string email)
    {
        if (token == null || email == null)
        {
            ModelState.AddModelError("", "Invalid password reset token.");
            return View("ResetPassword");
        }
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword(string email, string token, string newPassword, string confirmPassword)
    {
        if (newPassword != confirmPassword)
        {
            ModelState.AddModelError("", "Passwords do not match.");
            return View();
        }

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return View("ResetPasswordConfirmation");
        }

        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        if (result.Succeeded)
        {
            return View("ResetPasswordConfirmation");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }
        return View();
    }
    public IActionResult Logout()
    {

        _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }
}
