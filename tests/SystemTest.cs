using AProgram = agrix.Program.Program;
using FluentAssertions;
using MockHttp.Net;
using tests.Properties;
using Xunit;

namespace tests
{
    /// <summary>
    /// Tests provisioning via configuration.
    /// </summary>
    public class SystemTest
    {
        private const string ApiKey = "abc123";

        private const string StartupScriptListWithBuildIso = @"
{
    ""3"": {
        ""SCRIPTID"": ""3"",
        ""date_created"": ""2014-05-21 15:27:18"",
        ""date_modified"": ""2014-05-21 15:27:18"",
        ""name"": ""create-live-iso"",
        ""type"": ""boot"",
        ""script"": ""#!/bin/bash echo Hello World > /root/hello""
    }
}";

        private const string ExpectedBuildIsoScript =
            "SCRIPTID=3&script=%23%21%2Fusr%2Fbin%2Fenv+bash%0Abash+-c+%22%24%28curl+-fsSL+https%3A%2F%2Fraw.githubusercontent.com%2Fokinta%2Fvultr-scripts%2Fmaster%2Fcoreos%2Fcreate-live-iso.script.bash%29%22%0A";

        private const string ExpectedBuildIsoServer =
            "DCID=1&VPSPLANID=201&OSID=270&SCRIPTID=3&enable_private_network=no&label=isobuilder&userdata=eyJDTE9VREZMQVJFX0VNQUlMIjogIiRDTE9VREZMQVJFX0VNQUlMIiwgIkNMT1VERkxBUkVfUkVDT1JETkFNRSI6ICIkQ0xPVURGTEFSRV9SRUNPUkROQU1FIiwgIkNMT1VERkxBUkVfWk9ORU5BTUUiOiAiJENMT1VERkxBUkVfWk9ORU5BTUUiLCAiQ09OVEFJTkVSX1JFR0lTVFJZX0xPR0lOIjogIiRDT05UQUlORVJfUkVHSVNUUllfTE9HSU4iLCAiQ09OVEFJTkVSX1JFR0lTVFJZX05BTUUiOiAiJENPTlRBSU5FUl9SRUdJU1RSWV9OQU1FIiwgIkNPTlRBSU5FUl9SRUdJU1RSWV9QQVNTV09SRCI6ICIkQ09OVEFJTkVSX1JFR0lTVFJZX1BBU1NXT1JEIiwgIkxPR0ROQV9JTkdFU1RJT05fS0VZIjogIiRMT0dETkFfSU5HRVNUSU9OX0tFWSIsICJWVUxUUl9BUElfS0VZIjogIiRWVUxUUl9BUElfS0VZIn0%3D&notify_activate=no&FIREWALLGROUPID=1234abcd";

        private const string BuildServerResponse = "{\"SUBID\": \"1312965\"}";

        [Fact]
        public void TestBuildIso()
        {
            var input = new LineInputter(Resources.BuildIsoConfig);
            using var requests = new MockVultrRequests(
                new HttpHandler(
                    "account/info", Resources.VultrAccountInfo),
                new HttpHandler(
                    "startupscript/list",
                    new[]
                    {
                        StartupScriptListWithBuildIso, StartupScriptListWithBuildIso
                    }),
                new HttpHandler(
                    "startupscript/update",
                    ExpectedBuildIsoScript,
                    ""),
                new HttpHandler("os/list", Resources.VultrOSList),
                new HttpHandler(
                    "regions/list?availability=yes",
                    Resources.VultrRegionsList),
                new HttpHandler(
                    "plans/list?type=all", Resources.VultrPlansList),
                new HttpHandler(
                    "firewall/group_list", Resources.VultrFirewallGroupsPublic),
                new HttpHandler("server/list", "{}"),
                new HttpHandler(
                    "server/create",
                    ExpectedBuildIsoServer,
                    BuildServerResponse));

            AProgram.Main(null, input.ReadLine,
                "provision",
                "--apikey", ApiKey,
                "--apiurl", requests.Url).Should().Be(0);
            requests.AssertAllCalledOnce();
        }

        [Fact]
        public void TestBuildIsoDryrun()
        {
            var input = new LineInputter(Resources.BuildIsoConfig);
            using var requests = new MockVultrRequests(
                new HttpHandler(
                    "account/info", Resources.VultrAccountInfo),
                new HttpHandler(
                    "startupscript/list",
                    new[]
                    {
                        StartupScriptListWithBuildIso, StartupScriptListWithBuildIso
                    }),
                new HttpHandler("os/list", Resources.VultrOSList),
                new HttpHandler(
                    "regions/list?availability=yes",
                    Resources.VultrRegionsList),
                new HttpHandler(
                    "plans/list?type=all", Resources.VultrPlansList),
                new HttpHandler(
                    "firewall/group_list", Resources.VultrFirewallGroupsPublic),
                new HttpHandler("server/list", "{}"));

            AProgram.Main(null, input.ReadLine,
                "provision", "--dryrun",
                "--apikey", ApiKey,
                "--apiurl", requests.Url).Should().Be(0);
            requests.AssertAllCalledOnce();
        }

        [Fact]
        public void TestBuildIsoDryrunNoBuildScript()
        {
            var input = new LineInputter(Resources.BuildIsoConfig);
            using var requests = new MockVultrRequests(
                new HttpHandler(
                    "account/info", Resources.VultrAccountInfo),
                new HttpHandler(
                    "startupscript/list",
                    new[]
                    {
                        "{}", "{}"
                    }));

            AProgram.Main(null, input.ReadLine,
                "provision", "--dryrun",
                "--apikey", ApiKey,
                "--apiurl", requests.Url).Should().Be(3);
            requests.AssertAllCalledOnce();
        }
    }
}
