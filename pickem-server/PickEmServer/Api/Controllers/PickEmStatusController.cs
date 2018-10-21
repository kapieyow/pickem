
using Marten;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using PickEmServer.Api.Models;
using System.Diagnostics;
using System.Reflection;

namespace PickEmServer.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/status")]
    public class PickEmStatusController : Controller
    {
        private readonly IDocumentStore _documentStore;
        private readonly string _runtimeEnvironment = null;

        public PickEmStatusController(IHostingEnvironment env, IDocumentStore documentStore)
        {
            _documentStore = documentStore;
            _runtimeEnvironment = env.EnvironmentName;
        }

        // GET: api/status
        [HttpGet]
        public PickEmStatus Get()
        {
            PickEmStatus pickEmStatus = new PickEmStatus();

            // user if there is one
            if (User.Identity.IsAuthenticated)
            {
                pickEmStatus.AuthenticatedUserName = User.Identity.Name;
            }

            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(executingAssembly.Location);

            // TODO: may be a better way to do this. Creating a marten session to get the conn info
            using (var dbSession = _documentStore.LightweightSession())
            {
                pickEmStatus.Database = dbSession.Connection.Database;
                pickEmStatus.DatabaseHost = dbSession.Connection.Host;
            }
            
            pickEmStatus.Product = executingAssembly.GetCustomAttribute<AssemblyProductAttribute>().Product;
            pickEmStatus.ProductVersion = fileVersionInfo.ProductVersion;
            pickEmStatus.RuntimeEnvironment = _runtimeEnvironment;

            return pickEmStatus;
        }
    }
}