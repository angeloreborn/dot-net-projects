using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Chat_and_Mail_Services.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Drawing;

namespace Chat_and_Mail_Services.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ServicesUser> _signInManager;
        private readonly UserManager<ServicesUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _env;
        public RegisterModel(
            UserManager<ServicesUser> userManager,
            SignInManager<ServicesUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IWebHostEnvironment env)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _env = env;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [BindProperty]
            [Required]
            [Display(Name = "File")]
            public IFormFile FormFile { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }
        public bool ThumbnailCallback()
        {
            return false;
        }
        public class UploadLimit
        {
            public static int TWELVE_MEGABYTES = 12582912;
        }

        public class ImageSizes
        {
            public static BitmapDimensions Thumbnail = new()
            {
                X = 250,
                Y = 250
            };
        }

        public class BitmapDimensions
        {
            public int X { get; set; }
            public int Y { get; set; }
        }
        public async Task<OptimizedImage> ProcessImage(InputModel inputModel)
        {
            using (var memoryStream = new MemoryStream())
            {
                await inputModel.FormFile.CopyToAsync(memoryStream);
                if (memoryStream.Length < UploadLimit.TWELVE_MEGABYTES)
                {
                    Image _original = Image.FromStream(memoryStream);

                    double shrinkFactor = (_original.Width + _original.Height) / (ImageSizes.Thumbnail.X + ImageSizes.Thumbnail.Y);
                    BitmapDimensions desiredDimensions = new()
                    {
                        X = _original.Width / (int)shrinkFactor,
                        Y = _original.Height / (int)shrinkFactor
                    };
                    Image.GetThumbnailImageAbort thumbnailImageAbort = new Image.GetThumbnailImageAbort(ThumbnailCallback);
                    Image _shrunk = _original.GetThumbnailImage(desiredDimensions.X, desiredDimensions.Y, thumbnailImageAbort, IntPtr.Zero);
                    return new OptimizedImage
                    {
                        Original = _original,
                        Shrunk = _shrunk,
                    };
                }
                else return null; // Model state error management
            }
        }

        public void ByteArrayToImage()
        {
            
        }

        public class OptimizedImage
        {
            public Image Original { get; set; }
            public Image Shrunk { get; set; }

        }
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            OptimizedImage optimizedImage = await ProcessImage(Input);
            
            // Set image paths
            Guid imageGuid = Guid.NewGuid();
            string originPath = @"\wwwroot\images\origin\" + $"{imageGuid}.{optimizedImage.Original.RawFormat}";
            string shrunkPath = @"\wwwroot\images\thumbnail\" + $"{imageGuid}.{optimizedImage.Original.RawFormat}";

            // Save
            optimizedImage.Original.Save(_env.ContentRootPath + originPath, optimizedImage.Original.RawFormat);
            optimizedImage.Shrunk.Save(_env.ContentRootPath + shrunkPath, optimizedImage.Shrunk.RawFormat);

            if (ModelState.IsValid)
            {
                var user = new ServicesUser { UserName = Input.Email, Email = Input.Email, ProfilePhotoUid = imageGuid};
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
