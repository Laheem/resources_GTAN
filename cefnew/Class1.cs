using GTANetworkServer;

namespace cefnew
{
    public class Class1 : Script
    {
        public Class1()
        {
            API.onResourceStart += cefTestAlive;
        }

        private void cefTestAlive()
        {
            API.consoleOutput("Hello, CEF test alive.");
        }

        [Command("testcef")]
        public void Test(Client player)
        {
            API.triggerClientEvent(player, "showCefBrowser");
        }
    }
}