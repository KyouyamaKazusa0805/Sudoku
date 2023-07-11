var rootCommand = new RootCommand("A modern program that operates with sudoku puzzles using command line interface.");
rootCommand.AddCommand<GenerateCommand>();

return rootCommand.Invoke(args);
