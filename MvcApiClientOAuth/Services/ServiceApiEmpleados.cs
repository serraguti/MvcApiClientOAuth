using MvcApiClientOAuth.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;

namespace MvcApiClientOAuth.Services
{
    public class ServiceApiEmpleados
    {
        private string UrlApiEmpleados;
        private MediaTypeWithQualityHeaderValue Header;
        //OBJETO PARA RECUPERAR HttpContext Y EL User Y SU Claim
        private IHttpContextAccessor httpContextAccessor;

        public ServiceApiEmpleados
            (IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.UrlApiEmpleados =
                configuration.GetValue<string>("ApiUrls:ApiEmpleados");
            this.Header =
                new MediaTypeWithQualityHeaderValue("application/json");
        }

        public async Task<string> GetTokenAsync(string username
            , string password)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/auth/login";
                client.BaseAddress = new Uri(this.UrlApiEmpleados);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                LoginModel model = new LoginModel
                {
                    UserName = username, Password = password
                };
                string jsonData = JsonConvert.SerializeObject(model);
                StringContent content =
                    new StringContent(jsonData, Encoding.UTF8,
                    "application/json");
                HttpResponseMessage response = await
                    client.PostAsync(request, content);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    JObject keys = JObject.Parse(data);
                    string token = keys.GetValue("response").ToString();
                    return token;
                }
                else
                {
                    return null;
                }
            }
        }

        private async Task<T> CallApiAsync<T>(string request)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApiEmpleados);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                HttpResponseMessage response =
                    await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }

        //TENDREMOS UN METODO GENERICO QUE RECIBIRA EL REQUEST 
        //Y EL TOKEN
        private async Task<T> CallApiAsync<T>
            (string request, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApiEmpleados);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add
                    ("Authorization", "bearer " + token);
                HttpResponseMessage response =
                    await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }

        public async Task<List<Empleado>> GetEmpleadosAsync()
        {
            string request = "api/empleados";
            List<Empleado> empleados = await
                this.CallApiAsync<List<Empleado>>(request);
            return empleados;
        }

        //METODO PROTEGIDO
        public async Task<Empleado> FindEmpleadoAsync
            (int idEmpleado)
        {
            string token =
                this.httpContextAccessor
                .HttpContext.User
                .FindFirst(x => x.Type == "TOKEN").Value;
            string request = "api/empleados/" + idEmpleado;
            Empleado empleado = await
                this.CallApiAsync<Empleado>(request, token);
            return empleado;
        }
    }
}
