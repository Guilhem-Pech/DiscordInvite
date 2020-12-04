using System;

namespace DiscordInvite
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Too many arguments");
                Console.WriteLine("Usage: DiscordInvite <Token>");
                return 1;
            }
             
            Bot bot = new Bot(args[0]);
            bot.RunAsync().GetAwaiter().GetResult();
            return 0;
        }
    }
}