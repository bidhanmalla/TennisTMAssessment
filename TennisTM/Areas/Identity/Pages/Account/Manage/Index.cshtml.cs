// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TennisTM.Data;
using TennisTM.Models;

namespace TennisTM.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly TennisTMDbContext dbContext;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public IndexModel(
            TennisTMDbContext dbContext,
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            this.dbContext = dbContext;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

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
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            /// 
            [MaxLength(100)]
            [DataType(DataType.Text)]
            [Display(Name = "Bio")]
            public string Bio { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "Image")]
            public string Image { get; set; }

            [MaxLength(20)]
            [DataType(DataType.Text)]
            [Display(Name = "Name")]
            public string Name { get; set; }
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
        }

        private async Task LoadAsync(User user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var coach = await dbContext.Coaches.FirstOrDefaultAsync(x =>  x.UserId == user.Id);

            Username = userName;

            Input = new InputModel
            {
                Bio = coach.Bio,
                Image = coach.Image,
                Name = user.Name,
                PhoneNumber = phoneNumber
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            if (Input.Name != user.Name)
            {
                user.Name = Input.Name;
                var setNameResult = await dbContext.SaveChangesAsync();
                if (setNameResult != 1)
                {
                    StatusMessage = "Unexpected error when trying to set name.";
                    return RedirectToPage();
                }
            }

            var coach = await dbContext.Coaches.FirstOrDefaultAsync(x => x.UserId == user.Id);
            if (Input.Bio != coach.Bio && !string.IsNullOrEmpty(Input.Bio))
            {
                coach.Bio = Input.Bio;
                var setBioResult = await dbContext.SaveChangesAsync();
                if (setBioResult != 1)
                {
                    StatusMessage = "Unexpected error when trying to set bio.";
                    return RedirectToPage();
                }
            }

            if (Input.Image != coach.Image && !string.IsNullOrEmpty(Input.Image))
            {
                coach.Image = Input.Image;
                var setImageResult = await dbContext.SaveChangesAsync();
                if (setImageResult != 1)
                {
                    StatusMessage = "Unexpected error when trying to set image.";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
