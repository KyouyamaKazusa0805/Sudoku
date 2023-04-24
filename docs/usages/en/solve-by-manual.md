# Solve a puzzle

Goal: Using APIs to solve a sudoku puzzle

The method is simple. You should firstly get an encapsulated data of type `Grid`, and then pass it into the `Analyzer` instance to solve it.

```csharp
using System;
using Sudoku.Analytics;
using Sudoku.Concepts;

// This is a string text that represents a sudoku grid.
// The API supports many kinds of formats to describe a sudoku grid.
// Here I give you a default and simple sample.
const string puzzle = "........6.....158...8.4.21.5..8..39.6.1.7.8.5.89..5..1.24.5.9...659.....9........";
// Here you can also receive the sudoku grid data via console.
// string puzzle = args[0];

// Then parses the text, converting it into a valid sudoku grid.
var grid = (Grid)puzzle;
// You can also use 'Parse' method to get the Grid instance:
//var grid = Grid.Parse(puzzle);

// Initializes an analyzer instance, in order to solve the puzzle parsed above.
var analyzer = new Analyzer();

// Solves the puzzle synchronously.
// The method returns an instance of type 'AnalyzerResult',
// having encapsulated some data to describe the solving result,
// such as whether the puzzle has been solved successfully,
// the number of steps used, etc..
var analyzerResult = analyzer.Analyze(grid);

// Output the result.
// Here we just call the method 'ToString', the type has already overwritten the method.
// Also, in method Console.WriteLine, you can also omit 'ToString' invocation.
Console.WriteLine(analysisResult);
```

