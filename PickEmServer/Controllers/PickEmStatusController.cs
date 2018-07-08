
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using PickEmServer.ApiModels;
using System.Diagnostics;
using System.Reflection;

namespace PickEmServer.Controllers
{
    [Produces("application/json")]
    [Route("api/status")]
    public class PickEmStatusController : Controller
    {
        private string _runtimeEnvironment = null;

        public PickEmStatusController(IHostingEnvironment env)
        {
            _runtimeEnvironment = env.EnvironmentName;
        }

        // GET: api/status
        [HttpGet]
        public PickEmStatus Get()
        {
            PickEmStatus pickEmStatus = new PickEmStatus();

            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(executingAssembly.Location);

            pickEmStatus.Database = "OH CRAP. NO DB YET. TODO: set db";
            pickEmStatus.Product = executingAssembly.GetCustomAttribute<AssemblyProductAttribute>().Product;
            pickEmStatus.ProductVersion = fileVersionInfo.ProductVersion;
            pickEmStatus.RuntimeEnvironment = _runtimeEnvironment;

            return pickEmStatus;
        }
    }
}