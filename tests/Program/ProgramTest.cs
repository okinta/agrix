using AProgram = agrix.Program.Program;
using System.Reflection;
using Xunit;

namespace tests.Program
{
    public class ProgramTest
    {
        private readonly Assembly TestAssembly = Assembly.GetExecutingAssembly();

        [Fact]
        public void TestBadArguments()
        {
            Assert.Equal(2, AProgram.Main(TestAssembly, "bad", "arguments"));
        }

        [Fact]
        public void TestHelp()
        {
            Assert.Equal(0, AProgram.Main(TestAssembly, "help"));
        }
    }
}
