using agrix.Extensions;
using System.IO;
using System.Text;
using tests.Properties;
using YamlDotNet.RepresentationModel;

namespace tests.Configuration
{
    public class BaseTest
    {
        protected const string ApiKey = "abc123";

        protected YamlMappingNode LoadYaml()
        {
            var input = new StringReader(Encoding.Default.GetString(Resources.agrix));
            var yaml = new YamlStream();
            yaml.Load(input);
            return yaml.GetRootNode();
        }

        protected YamlMappingNode LoadYaml(string content)
        {
            var input = new StringReader(content);
            var yaml = new YamlStream();
            yaml.Load(input);
            return yaml.GetRootNode();
        }
    }
}
