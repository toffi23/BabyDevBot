using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BabyDevBot.Commands
{
    public class TestModule : ModuleBase<SocketCommandContext>
    {
        [Command("say")]
        [Summary("Repeats a message.")]
        public async Task SayAsync([Remainder][Summary("The message to repeat")]
                             string msg)
            => await ReplyAsync(msg);

        [Command("shine")]
        [Summary("Compliments a user.")]
        public async Task ShineAsync([Summary("The user to compliment")]
                               SocketUser user = null)
        {
            var target = user ?? Context.Message.Author;
            var text = ($"{target.Mention}, you are ugly as fuck... Ewww!");
            await Context.Message.Channel.SendMessageAsync(text);
        }
    }
}
