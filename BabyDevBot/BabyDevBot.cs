using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace BabyDevBot
{
    class BabyDevBot
    {
        private const string BotGuild = "Budapest Market Adoption @ RDI Budapest";
        private const string BotChannel = "bot-test";

        private DiscordSocketClient Client;
        private string Token { get; }

        private IBotMessageProvider Messages { get; set; }

        public BabyDevBot(string token)
        {
            Token = token;
        }

        public async Task Start()
        {
            try
            {
                Initialize();
                await Client.LoginAsync(TokenType.Bot, Token);
                await Client.StartAsync();

                await Task.Delay(-1);
            }
            catch(Exception ex)
            {
                await Log(new LogMessage(LogSeverity.Error, 
                                         "Exception",
                                         ex.Message, 
                                         ex));
            }
        }

        private void Initialize()
        {
            Client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = Discord.LogSeverity.Debug
            });
            Client.Log += Log;
            Client.MessageReceived += Client_MessageReceived;
            Client.Ready += Client_Ready;

            Messages = new BotMessagesHU();
        }

        private Task Client_Ready()
        {
           return SendWelcome();
        }

        private Task Client_MessageReceived(SocketMessage message)
        {
            if(message.Source != MessageSource.Bot)
            {
                ulong chid = message.Channel.Id;
                LogMsg($"Channel id: {chid}");
            }

            return Task.CompletedTask;
        }

        private Task Log(LogMessage message)
        {
            var msg = message.Exception != null ?
                      message.Exception.ToString() :
                      message.Message;

            LogMsg(msg, message.Source);
            return Task.CompletedTask;
        }

        private void LogMsg(string message, string source = "Internal")
        {
            var timeStamp = DateTime.Now.ToString("yyyy-dd-MM HH:mm:ss");
            Console.WriteLine($"[{timeStamp}][{source}] {message}");
        }

        private async Task SendWelcome()
        {
            var guild = Client.Guilds.First(g => g.Name == BotGuild);
            if (guild != null)
            {
                var channel = guild.Channels
                                   .First(ch => ch.Name == BotChannel)
                                    as ISocketMessageChannel;
                if (channel != null)
                {
                    LogMsg($"channel: {channel.Name}");
                    await channel.SendMessageAsync(Messages.GetWelcome());
                }
            }
        }
    }
}
