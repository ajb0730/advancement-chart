using Xunit.Abstractions;
using Xunit.Sdk;

[assembly: Xunit.TestFramework("advancement_chart.tests.Helpers.TestStartup", "advancement-chart.tests")]

namespace advancement_chart.tests.Helpers
{
    public class TestStartup : XunitTestFramework
    {
        public TestStartup(IMessageSink messageSink) : base(messageSink)
        {
            NativeLibraryResolver.Register();
        }
    }
}
