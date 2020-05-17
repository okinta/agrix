using agrix.Configuration;
using agrix.Extensions;
using tests.Properties;
using Xunit;

namespace tests.Configuration
{
    public class ParserTest : BaseTest
    {
        [Fact]
        public void TestParseEmpty()
        {
            var parser = new Parser();
            var node = LoadYaml("klouts:").GetNode("klouts");
            Assert.Empty(parser.Load("klouts", node, new KloutParser().Parse));
        }

        [Fact]
        public void TestParse()
        {
            var parser = new Parser();
            var node = LoadYaml(Resources.KloutsConfig).GetNode("klouts");

            var klouts = parser.Load("klouts", node, new KloutParser().Parse);
            Assert.Equal(3, klouts.Count);
            Assert.Equal(1, klouts[0].Score);
            Assert.Equal(99, klouts[1].Score);
            Assert.Equal(78, klouts[2].Score);
        }
    }
}
