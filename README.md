# Sudoku

A sudoku solver through brute forces and logical techniques (Update files continuously).



## Intro to Solution Folders

Here displays the introduction to all folders in this whole solution.



* Sudoku.Core.Old
  * The implementation of the sudoku meta data, such as sudoku grid class and so on. Also provides some extension methods in this project, such as string.Match([Pattern] string).
* Sudoku
  * The newer implementation of sudoku meta data. Different than Sudoku.Core.Old, all files are re-implemented by newer logic. For example, older implementation of grid class uses CellInfo struct to describe all information in a cell, which is a "vivid" description; while newer one uses bitwise operations (using 12 bits to represent a cell information, 3 bits for cell status and other 9 bits are candidates status).
* Sudoku.Debugging
  * The console program aiming to debugging codes logic of other projects.
* Sudoku.Diagnostics
  * The diagnostic controlling through all over the solution. In addition, those files are used with my own custom code analyzer and fixer (But this analyzer is not included in this solution. Therefore codes has not been uploaded).
* Sudoku.IO
  * I/O operations to sudoku data (This project has not been implemented).
* Sudoku.Solving.Bf.Bitwise
  * The bitwise brute force solver to a sudoku puzzle.
* Sudoku.Terminal
  * The terminal of this project. You can use console arguments (such as --solve to solve a grid). However, this project has not been implemented also.



## Intro to Files

Here displays the introduction to files in root folder.

* .editorconfig
  * Editor configuration file.
* Priority of operators.txt
  * Operators priority through C# language. (P.S. I don't know why I will upload this file, maybe of vital importance?)



## Author

Sunnie, a student from Chengdu, China... An extremely original undergraduate.
