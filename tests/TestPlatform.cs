using agrix.Configuration;
using agrix.Platforms;
using RestSharp;
using System;

namespace tests
{
    /// <summary>
    /// A non-functional platform used for testing purposes.
    /// </summary>
    [Platform("test", nameof(CreateTestPlatform))]
    internal class TestPlatform : Platform
    {
        private string ApiUrl { get; }

        public TestPlatform(string apiUrl)
        {
            if (string.IsNullOrEmpty(apiUrl))
                apiUrl = Environment.GetEnvironmentVariable("TEST_PLATFORM_API_URL");

            if (string.IsNullOrEmpty(apiUrl))
                throw new ArgumentNullException(
                    nameof(apiUrl), @"must not be empty");

            ApiUrl = apiUrl;
            AddProvisioner<Server>(ProvisionServer);
            AddDestroyer<Server>(DestroyServer);
        }

        public static IPlatform CreateTestPlatform(PlatformSettings settings)
        {
            return new TestPlatform(settings.ApiUrl);
        }

        public override void TestConnection()
        {
        }

        private void ProvisionServer(Server server, bool dryrun)
        {
            var client = new RestClient(ApiUrl);
            var request = new RestRequest("provision");
            request.AddParameter("plan.cpu", server.Plan.Cpu);
            request.AddParameter("plan.memory", server.Plan.Memory);
            request.AddParameter("plan.type", server.Plan.Type);
            request.AddParameter("os.name", server.Os.Name);
            request.AddParameter("os.app", server.Os.Name);
            request.AddParameter("os.iso", server.Os.Iso);
            request.AddParameter("dryrun", dryrun);
            client.Post(request);
        }

        private void DestroyServer(Server server, bool dryrun)
        {
            var client = new RestClient(ApiUrl);
            var request = new RestRequest("destroy");
            request.AddParameter("label", server.Label);
            request.AddParameter("dryrun", dryrun);
            client.Post(request);
        }
    }
}
