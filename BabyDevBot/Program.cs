using System;
using System.Reflection;
using System.Threading.Tasks;
using System.IO;

using Discord;
using Discord.WebSocket;
using Discord.Commands;

namespace BabyDevBot
{
    class Program
    {
        private DiscordSocketClient Client;
        private CommandService Commands;

        static void Main(string[] args)
        {
            try
            {
                (new Program()).MainAsync().GetAwaiter().GetResult();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        private async Task MainAsync()
        {
            Client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Debug
            });

            Commands = new CommandService(new CommandServiceConfig
            {
                CaseSensitiveCommands = false,
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Debug
            });
            
            Client.MessageReceived += Client_MessageReceived;
            await Commands.AddModulesAsync(Assembly.GetEntryAssembly(), services:null);

            Client.Ready += Client_Ready;
            Client.Log += Client_Log;

            string botToken = "";
            try
            {
                botToken = File.ReadAllText("BotToken.txt");
            }
            catch(Exception)
            {
                // Debug message comes here
            }
            await Client.LoginAsync(TokenType.Bot, botToken);
            await Client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task Client_Log(LogMessage msg)
        {
            LogMsg(msg);
        }

        private async Task Client_Ready()
        {
            await Client.SetStatusAsync(UserStatus.Online);
        }

        private async Task Client_MessageReceived(SocketMessage msg)
        {
            var message = msg as SocketUserMessage;
            if (message == null) return;

            int argPos = 0;

            if (!(message.HasCharPrefix('!', ref argPos) ||
                 message.HasMentionPrefix(Client.CurrentUser, ref argPos) ||
                 message.Author.IsBot))
                return;

            var context = new SocketCommandContext(Client, message);
            var result = await Commands.ExecuteAsync(context: context, argPos: argPos, services: null);
        }

        private void LogMsg(LogMessage msg)
        {
            Console.WriteLine($"[{DateTime.Now}][{msg.Source}] {msg}");
        }
    }
}
