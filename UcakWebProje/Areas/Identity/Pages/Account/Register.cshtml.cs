// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using UcakWebProje.Areas.Identity.Data;
using System.Security.Cryptography;

namespace UcakWebProje.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IUserStore<User> _userStore;
        private readonly IUserEmailStore<User> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<User> userManager,
            IUserStore<User> userStore,
            SignInManager<User> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            [Key]
            [Display(Name = "Username")]
            [Required(ErrorMessage = "This field is required!")]
            [RegularExpression("^[a-zA-Z0-9@.]+$", ErrorMessage = "Only letters, numbers, @ and . are allowed!")]
            [MaxLength(30, ErrorMessage = "Maximum length is {1} characters!")]
            public string UserName { get; set; }

            [Display(Name = "Password")]
            [Required(ErrorMessage = "This field is required!")]
            [RegularExpression("^((?=.*?[A-Z])|(?=.*?[a-z])|(?=.*?[0-9])|(?=.*?[#?!@$ %^&*-])).{3,}$", ErrorMessage = "Please enter a valid password!")]
            [DataType(DataType.Password)]
            [MaxLength(64, ErrorMessage = "Maximum length is {1} characters!")]
            public string Password { get; set; }

            [Display(Name = "First Name")]
            [Required(ErrorMessage = "This field is required!")]
            [RegularExpression("^[a-zA-Z][a-zA-Z\\s]{0,20}[a-zA-Z]$", ErrorMessage = "Please enter a valid name!")]
            [MaxLength(50, ErrorMessage = "Maximum length is {1} characters!")]
            public string FirstName { get; set; }

            [Display(Name = "Last Name")]
            [Required(ErrorMessage = "This field is required!")]
            [RegularExpression("^[a-zA-Z][a-zA-Z\\s]{0,20}[a-zA-Z]$", ErrorMessage = "Please enter a valid name!")]
            [MaxLength(50, ErrorMessage = "Maximum length is {1} characters!")]
            public string LastName { get; set; }

            [Display(Name = "E-Mail")]
            [Required(ErrorMessage = "This field is required!")]
            [EmailAddress(ErrorMessage = "Please enter a valid mail address!")]
            [MaxLength(80, ErrorMessage = "Maximum length is {1} characters!")]
            public string Mail { get; set; }

            [Display(Name = "Phone")]
            [Required(ErrorMessage = "This field is required!")]
            [Phone]
            [RegularExpression("^[\\+]?[(]?[0-9]{3}[)]?[-\\s\\.]?[0-9]{3}[-\\s\\.]?[0-9]{4,6}$", ErrorMessage = "Please enter a valid phone number!")]
            [MaxLength(20, ErrorMessage = "Maximum length is {1} characters!")]
            public string phoneNum { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                StringBuilder sb = new StringBuilder();
                using (SHA256 sha256Hash = SHA256.Create())
                {
                    byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(HttpContext.Request.Form["Password"].ToString()));

                    for (int i = 0; i < bytes.Length; i++)
                    {
                        sb.Append(bytes[i].ToString("x2"));
                    }
                }

                user.UserName = Input.UserName;
                user.Password = sb.ToString();
                user.FirstName = Input.FirstName;
                user.LastName = Input.LastName;
                user.Mail = Input.Mail;
                user.phoneNum = Input.phoneNum;

                await _userStore.SetUserNameAsync(user, Input.UserName, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.UserName, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, sb.ToString());

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Mail, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Mail, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        if (HttpContext.Request.Cookies["travel"] is not null)
                        {
                            return RedirectToAction("BuyTicket", "Home", new { area = "" });
                        }
                        return LocalRedirect(returnUrl);
                    }
                }
                /*
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                */
            }

            // If we got this far, something failed, redisplay form
            TempData["signupFailed"] = 1;
            return Page();
        }

        private User CreateUser()
        {
            try
            {
                return Activator.CreateInstance<User>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(User)}'. " +
                    $"Ensure that '{nameof(User)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<User> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<User>)_userStore;
        }
    }
}
