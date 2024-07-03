using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModelClasses;
using ModelClasses.ViewModel;

namespace MMLTongaShop.Controllers
{

    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<AccountController> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<AccountController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginVM vm = new LoginVM();
            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginVM login)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogInformation("ModelState is invalid.");
                return View(login);
            }

            _logger.LogInformation("Attempting to log in user {Email}", login.Email);
            var user = await _userManager.FindByEmailAsync(login.Email);
            if (user != null)
            {
                // Log the normalized email
                var normalizedInputEmail = _userManager.NormalizeEmail(login.Email);
                _logger.LogInformation($"Normalized email from UserManager: {normalizedInputEmail}");
                _logger.LogInformation($"Normalized email from database for user {user.UserName}: {user.NormalizedEmail}");

                // Proceed with login attempt
                try
                {
                    var result = await _signInManager.PasswordSignInAsync(user.UserName, login.Password, isPersistent: false, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User {Email} logged in successfully.", login.Email);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        if (result.IsLockedOut)
                        {
                            _logger.LogWarning("User {Email} account locked out.", login.Email);
                            ModelState.AddModelError(string.Empty, "User account locked.");
                        }
                        else if (result.IsNotAllowed)
                        {
                            _logger.LogWarning("User {Email} login not allowed.", login.Email);
                            ModelState.AddModelError(string.Empty, "Login not allowed.");
                        }
                        else if (result.RequiresTwoFactor)
                        {
                            _logger.LogInformation("User {Email} requires two-factor authentication.", login.Email);
                            ModelState.AddModelError(string.Empty, "Requires two-factor authentication.");
                        }
                        else
                        {
                            _logger.LogWarning("Invalid login attempt for user {Email}.", login.Email);
                            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "User {Email} login failed due to an exception.", login.Email);
                    ModelState.AddModelError(string.Empty, "An error occurred while processing your request.");
                }
            }
            else
            {
                _logger.LogWarning("Login attempt failed. User with email {Email} not found.", login.Email);
                ModelState.AddModelError(string.Empty, "User does not exist.");
            }

            return View(login);
        }


        public IActionResult Register()
        {
            RegisterVM vm = new RegisterVM();
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM register)
        {
            var user = new ApplicationUser
            {
                FirstName = register.applicationUser.FirstName,
                LastName = register.applicationUser.LastName,
                Email = register.Email,
                NormalizedEmail = _userManager.NormalizeEmail(register.Email), //I added
                UserName = register.UserName,
                NormalizedUserName = _userManager.NormalizeName(register.UserName), ///I added
                Address = register.applicationUser.Address,
            };

            var Registration = await _userManager.CreateAsync(user, register.Password);

            if (Registration.Succeeded)
            {
                // Check if the registered email is for the admin
                if (user.Email.Equals("admin@gmail.com", StringComparison.OrdinalIgnoreCase))
                {
                    // Assign the "Admin" role
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
                else
                {
                    // Assign the "User" role or any other default role
                    await _userManager.AddToRoleAsync(user, "User");
                }

                await _signInManager.SignInAsync(user, isPersistent: false);
                register.StatusMessage = "Registered Successfully!";
                return RedirectToAction("Index", "Home");
            }
            else
            {

                foreach (var error in Registration.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(register);


            }

        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
