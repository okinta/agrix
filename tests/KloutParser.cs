using agrix.Extensions;
using YamlDotNet.RepresentationModel;

namespace tests
{
    internal struct Klout
    {
        public int Score { get; set; }
    }

    internal class KloutParser
    {
        public Klout Parse(YamlNode node)
        {
            var item = (YamlMappingNode)node;
            return new Klout() { Score = int.Parse(item.GetKey("klout")) };
        }
    }
}
