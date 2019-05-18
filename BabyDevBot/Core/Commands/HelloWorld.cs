using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BabyDevBot.Core.Commands
{
    public class HelloWorld : ModuleBase<SocketCommandContext>
    {
        [Command("hello"), Alias("szia", "mizu", "csumi"), Summary("greeting")]
        public async Task GreetingMyFriend()
        {
            var msg = Context.Message;
            var sender = msg.Author.Username;
            await Context.Channel.SendMessageAsync($"Szia {sender}! Vidéki vagy?");
        }
    }
}
