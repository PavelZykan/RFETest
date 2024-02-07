using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace RFETest.WebApi.IntegrationTests
{
    public partial class ApiIntegrationTests
    {
        internal class ApplicationUnderTest : WebApplicationFactory<Program>
        {
            protected override IHost CreateHost(IHostBuilder builder)
            {
                return base.CreateHost(builder);
            }
        }
    }
}
