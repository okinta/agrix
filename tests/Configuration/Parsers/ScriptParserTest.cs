using agrix.Configuration.Parsers;
using agrix.Configuration;
using agrix.Extensions;
using System;
using tests.Properties;
using Xunit;

namespace tests.Configuration.Parsers
{
    public class ScriptParserTest : BaseTest
    {
        [Fact]
        public void TestParse()
        {
            var scripts = LoadYaml(Resources.ScriptsConfig).GetSequence("scripts");
            var script = new ScriptParser().Parse(scripts[0]);
            Assert.Equal("test", script.Name);
            Assert.Equal(ScriptType.Boot, script.Type);
            Assert.Equal("this is a test script", script.Content);
        }

        [Fact]
        public void TestParse2()
        {
            var scripts = LoadYaml(Resources.ScriptsConfig).GetSequence("scripts");
            var script = new ScriptParser().Parse(scripts[1]);
            Assert.Equal("bash-script", script.Name);
            Assert.Equal(ScriptType.Boot, script.Type);
            Assert.Equal(string.Join('\n',
                "#!/usr/bin/env bash",
                "echo hello"), script.Content);
        }

        [Fact]
        public void TestParseInvalidTypeScript()
        {
            var scripts = LoadYaml(Resources.InvalidScriptTypeConfig)
                .GetSequence("scripts");

            Assert.Throws<ArgumentException>(() => new ScriptParser().Parse(scripts[0]));
        }
    }
}
