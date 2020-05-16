using AProgram = agrix.Program.Program;
using System.Reflection;
using tests.Properties;
using Xunit;

namespace tests.Program
{
    public class ProgramTest
    {
        private readonly Assembly TestAssembly = Assembly.GetExecutingAssembly();

        [Fact]
        public void TestBadArguments()
        {
            Assert.Equal(2, AProgram.Main(TestAssembly, ReadLine, "bad", "arguments"));
        }

        [Fact]
        public void TestHelp()
        {
            Assert.Equal(0, AProgram.Main(TestAssembly, ReadLine, "help"));
        }

        [Fact]
        public void TestValidate()
        {
            var input = new LineInputter(Resources.TestPlatformConfig);
            Assert.Equal(0, AProgram.Main(TestAssembly, input.ReadLine,
                "validate", "--apikey", "abc"));
        }

        [Fact]
        public void TestValidateInvalid()
        {
            var input = new LineInputter("what is this nonsense");
            Assert.Equal(1, AProgram.Main(TestAssembly, input.ReadLine,
                "validate", "--apikey", "abc"));
        }

        private string ReadLine() { return null; }
    }
}
