using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SpotifyAPI.Web;
using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace PerakladBot.Commands
{
    public static class TranslationCommand
    {
        public static void Translate(IWebDriver webDriver, TwitchClient client, ChatMessage chatMessage, string searchTerm, string translationType)
        {
            webDriver.FindElement(By.Id(translationType));

            IWebElement searchBox = webDriver.FindElement(By.Name("term"));
            searchBox.SendKeys(searchTerm);
            searchBox.Submit();

            WebDriverWait wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(120));
            wait.Until(d => d.Title.ToLower().StartsWith(searchTerm, StringComparison.CurrentCultureIgnoreCase));

            client.SendMessage(client.JoinedChannels.First(), $"@{chatMessage.DisplayName}: пераклад слова \"{searchTerm}\" на экране!");
        }
    }
}