using Microsoft.AspNetCore.Mvc;
using MvcApiClientOAuth.Filters;
using MvcApiClientOAuth.Models;
using MvcApiClientOAuth.Services;
using System.Security.Claims;

namespace MvcApiClientOAuth.Controllers
{
    public class EmpleadosController : Controller
    {
        private ServiceApiEmpleados service;

        public EmpleadosController(ServiceApiEmpleados service)
        {
            this.service = service;
        }

        [AuthorizeEmpleados]
        public async Task<IActionResult> Index()
        {
            List<Empleado> empleados = await
                this.service.GetEmpleadosAsync();
            return View(empleados);
        }

        [AuthorizeEmpleados]
        public async Task<IActionResult> Details(int id)
        {
            Empleado empleado = await
                    this.service.FindEmpleadoAsync(id);
            return View(empleado);
        }
    }
}
