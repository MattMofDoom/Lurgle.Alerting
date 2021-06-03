using Lurgle.Alerting;

namespace LurgleTest
{
    internal class Program
    {
        private static void Main()
        {
            Alert.From("Bob@Builder.com").Subject("Test").Send("Can you fix it?");
        }
    }
}
