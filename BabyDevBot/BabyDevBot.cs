using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using System.Linq;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace BabyDevBot
{
    class BabyDevBot
    {
        private DiscordSocketClient Client;
        private CommandService Commands;
        private CommandHandler CommandHandler;
        private IServiceProvider ServiceProvider;
        private readonly string Token;
        private IBotMessageProvider Messages;

        public LogSeverity LogLevel;

        public BabyDevBot(string token, LogSeverity logSeverity = LogSeverity.Warning)
        {
            Token = token;
            LogLevel = logSeverity;
        }

        public async Task Start()
        {
            try
            {
                await Initialize();
                await Client.LoginAsync(TokenType.Bot, Token);
                await Client.StartAsync();

                await Task.Delay(-1);
            }
            catch (Exception ex)
            {
                await Log(new LogMessage(LogSeverity.Error,
                                         "Exception",
                                         ex.Message,
                                         ex));
            }
        }

        private async Task Initialize()
        {
            Client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogLevel
            });
            Client.Log += Log;
            Client.MessageReceived += Client_MessageReceived;
            Client.Ready += Client_Ready;

            Commands = new CommandService(new CommandServiceConfig
            {
                CaseSensitiveCommands = false,
                LogLevel = LogLevel,
            });

            ServiceProvider = BuildServiceProvider();
            CommandHandler = new CommandHandler(Client, Commands, ServiceProvider);
            await CommandHandler.InstallCommandsAsync();

            Messages = new BotMessagesHU();            
        }

        private IServiceProvider BuildServiceProvider() => new ServiceCollection()
            .AddSingleton(Client)
            .AddSingleton(Commands)
            .AddSingleton<CommandHandler>()
            .BuildServiceProvider();

        private Task Client_Ready()
        {
           return SendWelcome();
        }

        private Task Client_MessageReceived(SocketMessage message)
        {
            if(message.Source != MessageSource.Bot)
            {
                // Do nothing for now 
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
            var channel = GetBotTestChannel();
            if (channel != null)
            {
                LogMsg($"channel: {channel.Name}");
                await channel.SendMessageAsync(Messages.GetWelcome());
            }
        }

        // TODO: implement generic Maybe pattern to avoid returning null
        private ISocketMessageChannel GetBotTestChannel()
        {
            const string botGuild = "Budapest Market Adoption @ RDI Budapest";
            const string botChannel = "bot-test";
            ISocketMessageChannel channel = null;

            var guild = Client.Guilds.First(g => g.Name == botGuild);
            if (guild != null)
            {
                channel = guild.Channels
                               .First(ch => ch.Name == botChannel)
                                as ISocketMessageChannel;
            }

            return channel;
        }
    }
}
