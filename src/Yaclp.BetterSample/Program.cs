﻿namespace Yaclp.BetterSample
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var configuration = DefaultParser.ParseOrExitWithUsageMessage<CommandLineParameters>(args);

            new Greeter(configuration).SayHello();
        }
    }
}