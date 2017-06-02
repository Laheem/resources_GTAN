using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GTANetworkServer;

namespace Accent
{
    public class AccentServer : Script
    {
        private readonly List<Accent> allAccents = new List<Accent>();
        private readonly string filePath = "afilepath";

        private DateTime lastCheck;

        public AccentServer()
        {
            API.onResourceStart += accentStarted;
            API.onUpdate += backupAccentFile;
            API.onClientEventTrigger += accentChoice;
            API.onChatMessage += appendAccent;
            API.onPlayerConnected += defaultAccent;
        }

        // Sets the first accent to a player if they don't have any entity data for an Accent already.
        private void defaultAccent(Client player)
        {
            if (!API.hasEntityData(player, "accent"))
            {
                API.setEntityData(player, "accent", allAccents.First());
                API.setEntityData(player, "helperaccent", true);
            }
        }
        
        // Checks to see if a player has the helper available, or just the message with the Accent Name appended on.
        private void appendAccent(Client sender, string message, CancelEventArgs cancel)
        {
            Accent a = API.getEntityData(sender, "accent");
            if (API.getEntityData(sender, "helperaccent"))
            {
                sendMessageInRadius(sender, 30f, sender.name + ": " + a + " " + a.replaceWords(message));
                cancel.Cancel = true;
                return;
            }
            sendMessageInRadius(sender, 30f, sender.name + ": " + a + " " + message);
            cancel.Cancel = true;
        }

        // Gets the accentString from the client side, then finds said Accent in the list of all.
        private void accentChoice(Client sender, string eventName, params object[] arguments)
        {
            if (eventName == "accentChange")
            {
                var a = findAccent(arguments[0].ToString());
                if (a != null)
                    API.setEntityData(sender, "accent", a);
            }
        }

        // NOT CURRENTLY WORKING. SEE FORUM POST.
        private void backupAccentFile()
        {
            if (DateTime.Now.Subtract(lastCheck).TotalMinutes >= 60)
            {
                /* lastCheck = DateTime.Now;
                 using (FileStream stream = new FileStream(filePath, FileMode.Create))
                 {
                     IFormatter formatter = new BinaryFormatter();
                     formatter.Serialize(stream, allAccents);
                     stream.Close();
                     
                 }
                 */
            }
        }

        // NOT CURRENTLY WORKING. SEE FORUM POST.
        private void accentStarted()
        {
            API.consoleOutput("Accents have started! Never forget the gangster accent.");
            if (File.Exists(filePath))
            {
                /*   using (FileStream stream = new FileStream(filePath, FileMode.Open))
                   {
                       BinaryFormatter formatter = new BinaryFormatter();
                       allAccents = formatter.Deserialize(stream) as List<Accent>;
                       stream.Close();
                   }
   */
            }

            allAccents.Add(new Accent("English"));
            allAccents.Add(new Accent("American"));
        }

        // Adds an accent to the list. Does checks to ensure the accent doesn't exist already and is above a certain length.
        [Command("addaccent", GreedyArg = true)]
        public void addAccent(Client sender, string accentToAdd)
        {
            if (findAccent(accentToAdd) == null && accentToAdd.Length >= 3)
            {
                var a = new Accent(accentToAdd);
                allAccents.Add(a);
                API.sendNotificationToPlayer(sender, "You have added the " + a.accentName + " Accent!");

                return;
            }
            API.sendNotificationToPlayer(sender, "That accent already exists!");
        }

        // Calls the client side, to choose from a dynamic list of Accents based on all the ones currently available.
        [Command("accent")]
        public void chooseAccent(Client sender)
        {
            API.triggerClientEvent(sender, "accentchoice", returnAllAccentNames(allAccents));
        }

        public void sendMessageInRadius(Client sender, float distance, string msg)
        {
            var playerList = API.getPlayersInRadiusOfPlayer(distance, sender);
            foreach (var p in playerList)
                API.sendChatMessageToPlayer(p, msg);
        }

        // Helper method, that parses the accentNames into a mapping that can be used client side.
        public List<string> returnAllAccentNames(List<Accent> listofAccents)
        {
            var allAccentNames = new List<string>();
            foreach (var a in listofAccents)
            {
                var jsonParse = new Dictionary<string, object>();
                jsonParse["accentName"] = a.accentName;
                allAccentNames.Add(API.toJson(jsonParse));
            }

            return allAccentNames;
        }

        // Adds a new "helper" word into the map, which will automatically change any word said by the user.
        [Command("addnewWord", GreedyArg = true)]
        public void addwordToChange(Client sender, string targetAccent, string wordToChange, string newWord)
        {
            var a = findAccent(targetAccent);
            string s;
            if (a != null && !a.wordsToChange.TryGetValue(wordToChange.ToLower(), out s))
            {
                a.wordsToChange.Add(wordToChange.ToLower(), newWord.ToLower());
                API.sendNotificationToPlayer(sender,
                    "Word : " + wordToChange.ToLower() + " will now be replaced by " + newWord.ToLower());
                return;
            }

            API.sendNotificationToPlayer(sender,
                "Unable to add a word, this accent may not exist or the word already exists.");
        }

        // Toggles the helper, preventing the system from changing the words
        [Command("helpertog")]
        public void togHelper(Client sender)
        {
            if (API.getEntityData(sender, "helperaccent"))
            {
                API.sendNotificationToPlayer(sender, "Helper toggled off.");
                API.setEntityData(sender, "helperaccent", false);
            }
            else
            {
                API.sendNotificationToPlayer(sender, "Helper toggled on.");
                API.setEntityData(sender, "helperaccent", true);
            }
        }


        // Helper method, which is used for checking if an accent already exists in the allAccents list.
        public Accent findAccent(string targetAccent)
        {
            foreach (var a in allAccents)
                if (targetAccent.ToLower().Equals(a.accentName.ToLower()))
                    return a;

            return null;
        }
    }
}