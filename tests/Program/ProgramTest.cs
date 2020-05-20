using AProgram = agrix.Program.Program;
using MockHttp.Net;
using System.Reflection;
using tests.Properties;
using Xunit;

namespace tests.Program
{
    public class ProgramTest
    {
        private readonly Assembly _testAssembly = Assembly.GetExecutingAssembly();

        [Fact]
        public void TestBadArguments()
        {
            Assert.Equal(
                2,
                AProgram.Main(_testAssembly, ReadLine, "bad", "arguments"));
        }

        [Fact]
        public void TestHelp()
        {
            Assert.Equal(
                0,
                AProgram.Main(_testAssembly, ReadLine, "help"));
        }

        [Fact]
        public void TestValidate()
        {
            var input = new LineInputter(Resources.TestPlatformConfig);
            Assert.Equal(0, AProgram.Main(_testAssembly, input.ReadLine,
                "validate", "--apiurl", "http://example.org/"));
        }

        [Fact]
        public void TestValidateInvalid()
        {
            var input = new LineInputter("what is this nonsense");
            Assert.Equal(1, AProgram.Main(_testAssembly, input.ReadLine,
                "validate", "--apikey", "abc"));
        }

        [Fact]
        public void TestProvision()
        {
            const string expected = "plan.cpu=2&plan.memory=4096&plan.type=SSD&" +
                                    "os.name=Fedora%2032%20x64&os.app=Fedora%2032%20x64&" +
                                    "os.iso=&dryrun=False";
            using var requests = new MockVultrRequests(
                new HttpHandler(
                    "provision", expected, ""));
            var input = new LineInputter(Resources.TestPlatformConfig);
            Assert.Equal(0, AProgram.Main(_testAssembly, input.ReadLine,
                "provision", "--apiurl", requests.Url));
            requests.AssertAllCalledOnce();
        }

        [Fact]
        public void TestProvisionDryrun()
        {
            const string expected = "plan.cpu=2&plan.memory=4096&plan.type=SSD&" +
                                    "os.name=Fedora%2032%20x64&os.app=Fedora%2032%20x64&" +
                                    "os.iso=&dryrun=True";
            using var requests = new MockVultrRequests(
                new HttpHandler(
                    "provision", expected, ""));
            var input = new LineInputter(Resources.TestPlatformConfig);
            Assert.Equal(0, AProgram.Main(_testAssembly, input.ReadLine,
                "provision", "--apiurl", requests.Url, "--dryrun"));
            requests.AssertAllCalledOnce();
        }

        private static string ReadLine() { return null; }
    }
}
