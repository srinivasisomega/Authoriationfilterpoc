// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;


using Identity_final_attempt.Data;
using Microsoft.EntityFrameworkCore; // Required for ToListAsync()

public class RegisterModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IAuthenticationSchemeProvider _schemeProvider;

    public RegisterModel(UserManager<ApplicationUser> userManager,
                         SignInManager<ApplicationUser> signInManager,
                         RoleManager<IdentityRole> roleManager,
                         IAuthenticationSchemeProvider schemeProvider)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _schemeProvider = schemeProvider;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public List<IdentityRole> Roles { get; set; } // For role selection

    public IList<AuthenticationScheme> ExternalLogins { get; set; } // For external logins

    // Add ReturnUrl property
    public string ReturnUrl { get; set; }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string RoleId { get; set; } // Selected role ID
    }

    public async Task OnGetAsync(string returnUrl = null) // Accept returnUrl as a parameter
    {
        ReturnUrl = returnUrl; // Assign the parameter to the property

        // Populate available roles for the dropdown
        Roles = await _roleManager.Roles.ToListAsync();

        // Populate external logins
        ExternalLogins = (await _schemeProvider.GetAllSchemesAsync())
            .Where(scheme => scheme.Name != "Cookies") // Exclude the cookies scheme
            .ToList();
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser { UserName = Input.Email, Email = Input.Email };
            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                // Assign the selected role to the user
                if (!string.IsNullOrEmpty(Input.RoleId))
                {
                    var role = await _roleManager.FindByIdAsync(Input.RoleId);
                    if (role != null)
                    {
                        await _userManager.AddToRoleAsync(user, role.Name);
                    }
                }

                await _signInManager.SignInAsync(user, isPersistent: false);
                return LocalRedirect(returnUrl ?? Url.Content("~/"));
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        // Reload roles and external logins if registration fails
        Roles = await _roleManager.Roles.ToListAsync();
        ExternalLogins = (await _schemeProvider.GetAllSchemesAsync())
            .Where(scheme => scheme.Name != "Cookies")
            .ToList();

        return Page();
    }
}

