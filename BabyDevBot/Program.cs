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
        static void Main(string[] args)
        {
            try
            {
                var p = new Program();
                if(p.Bot != null)
                    p.Bot.Start().GetAwaiter().GetResult();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public BabyDevBot Bot;
        private const string TokenFileName = "token";

        public Program()
        {
            try
            {
                if(File.Exists(TokenFileName))
                {
                    var token = File.ReadAllText(TokenFileName);
                    Bot = new BabyDevBot(token);
                }
                else
                {
                    Console.Error.WriteLine("Error: missing token file. Maybe you forget to put the file \"token\" here.");
                }
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }
    }
}
