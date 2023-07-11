var rootCommand = new RootCommand("Sample application using System.CommandLine official API.");
rootCommand.AddCommand(ICommand<GenerateCommand>.CreateCommand());

return rootCommand.Invoke(args);
