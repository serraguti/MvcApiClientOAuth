using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MvcApiClientOAuth.Models;
using MvcApiClientOAuth.Services;
using System.Security.Claims;

namespace MvcApiClientOAuth.Controllers
{
    public class ManagedController : Controller
    {
        private ServiceApiEmpleados service;

        public ManagedController(ServiceApiEmpleados service)
        {
            this.service = service;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login
            (LoginModel model)
        {
            string token = await this.service
                .GetTokenAsync(model.UserName, model.Password);
            if (token == null)
            {
                ViewData["MENSAJE"] = "Usuario/Password incorrectos";
                return View();
            }
            else
            {
                HttpContext.Session.SetString("TOKEN", token);
                ClaimsIdentity identity =
                    new ClaimsIdentity
                    (CookieAuthenticationDefaults.AuthenticationScheme
                    , ClaimTypes.Name, ClaimTypes.Role);
                //ALMACENAMOS EL NOMBRE DE USUARIO (BONITO)
                identity.AddClaim
                    (new Claim(ClaimTypes.Name, model.UserName));
                //ALMACENAMOS EL ID DEL USUARIO
                identity.AddClaim
    (new Claim(ClaimTypes.NameIdentifier, model.Password));
                identity.AddClaim
                    (new Claim("TOKEN", token));
                ClaimsPrincipal userPrincipal =
                    new ClaimsPrincipal(identity);
                //DAMOS DE ALTA AL USUARIO INDICANDO QUE 
                //ESTARA VALIDADO DURANTE 30 MINUTOS
                await HttpContext.SignInAsync
                    (CookieAuthenticationDefaults.AuthenticationScheme
                    , userPrincipal, new AuthenticationProperties
                    {
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                    });
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync
                (CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
