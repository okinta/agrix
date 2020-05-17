using AProgram = agrix.Program.Program;
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
                "validate", "--apikey", "abc"));
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
            var input = new LineInputter(Resources.TestPlatformConfig);
            Assert.Equal(0, AProgram.Main(_testAssembly, input.ReadLine,
                "provision", "--apikey", "abc"));

            var platform = TestPlatform.LastInstance;
            Assert.Equal(1, platform.Provisions.Count);

            var server = platform.Provisions[0].Item1;
            Assert.Equal("Fedora 32 x64", server.Os.Name);
            Assert.False(platform.Provisions[0].Item2);
        }

        [Fact]
        public void TestProvisionDryrun()
        {
            var input = new LineInputter(Resources.TestPlatformConfig);
            Assert.Equal(0, AProgram.Main(_testAssembly, input.ReadLine,
                "provision", "--apikey", "abc", "--dryrun"));

            var platform = TestPlatform.LastInstance;
            Assert.Equal(1, platform.Provisions.Count);

            var server = platform.Provisions[0].Item1;
            Assert.Equal("Fedora 32 x64", server.Os.Name);
            Assert.True(platform.Provisions[0].Item2);
        }

        private static string ReadLine() { return null; }
    }
}
