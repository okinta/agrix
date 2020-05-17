using agrix.Configuration;
using agrix.Extensions;
using tests.Properties;
using Xunit;
using YamlDotNet.RepresentationModel;

namespace tests.Configuration
{
    internal struct TestParser
    {
        public int Klout { get; set; }
    }

    public class ParserTest : BaseTest
    {
        [Fact]
        public void TestParseEmpty()
        {
            var parser = new Parser();
            var node = LoadYaml("klouts:").GetNode("klouts");
            Assert.Empty(parser.Load("klouts", node, Parse));
        }

        [Fact]
        public void TestParse()
        {
            var parser = new Parser();
            var node = LoadYaml(Resources.KloutsConfig).GetNode("klouts");

            var klouts = parser.Load("klouts", node, Parse);
            Assert.Equal(3, klouts.Count);
            Assert.Equal(1, klouts[0].Klout);
            Assert.Equal(99, klouts[1].Klout);
            Assert.Equal(78, klouts[2].Klout);
        }

        private TestParser Parse(YamlNode node)
        {
            var item = (YamlMappingNode)node;
            return new TestParser() { Klout = int.Parse(item.GetKey("klout")) };
        }
    }
}
