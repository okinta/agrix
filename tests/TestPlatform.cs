using agrix.Configuration;
using agrix.Platforms;
using YamlDotNet.RepresentationModel;

namespace tests
{
    /// <summary>
    /// A non-functional platform used for testing purposes.
    /// </summary>
    [Platform("test")]
    internal class TestPlatform : IPlatform
    {
        public TestPlatform(string _) { }

        public Infrastructure Load(YamlMappingNode yaml)
        {
            throw new System.NotImplementedException();
        }

        public void Provision(Infrastructure server, bool dryrun = false)
        {
            throw new System.NotImplementedException();
        }

        public void TestConnection()
        {
            throw new System.NotImplementedException();
        }
    }
}
