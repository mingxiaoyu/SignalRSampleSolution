using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebChat.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string UserName { get; set; }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            string userId = Guid.NewGuid().ToString().Replace("-", "");
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name,UserName),   //储存用户name
                new Claim(ClaimTypes.NameIdentifier,UserName)  //储存用户id
            };
            //身份【似身份证，多个声明（姓名，民族等）构成】
            var indentity = new ClaimsIdentity(claims, "formlogin");
            //证件所有者【类似身份证所有者】
            var principal = new ClaimsPrincipal(indentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            return RedirectToPage("/Index");

        }
    }
}