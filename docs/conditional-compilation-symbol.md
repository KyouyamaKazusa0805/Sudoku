# Conditional Compilation Symbol

Here display all conditional compilation symbols (CCS) in this solution. CCSes are the global Boolean values that exist in some projects to indicate whether the code block with these symbols are compiled or not. The block can't be compiled until the specified symbol is set in the project file (`*.csproj`).

Some of them are unnecessary for you perhaps, you can remove them. In addition, if you want to modify any CCSes, please search for ways online on modifying them.

| CCS                          | Default | Usage                                                        |
| ---------------------------- | ------- | ------------------------------------------------------------ |
| `DEBUG`                      | `true`  | Indicates the current environment is for debugging.          |
| `SUDOKU_RECOGNITION`         | `true`  | Indicates whether your machine can use OCR tools to recognize an image, and convert to a sudoku grid data structure instance. |
| `AUTHOR_RESERVED`            | `true`  | Indicates the method is only used for author himself. You can delete the code surrounded with this symbol. |
| `MUST_DOWNLOAD_TRAINED_DATA` | `false` | Indicates whether the solution will download the trained data file `eng.traineddata` on GitHub when the local file with the same name cannot be found. Sometimes the file downloading is too slow to stand with it. If this symbol is undefined, it'll offer the user an error message window,  saying local file cannot be found. |
| `DOUBLE_LAYERED_ASSUMPTION`  | `false` | Indicates whether projects will enable double-layered assumption to solve all puzzles. Double-layered assumption is a very terrible solving technique to solve all sudoku puzzles. They're copied from Sudoku Explainer project.<br />Please note that this feature is being under construction, I can't decide whether I'll update and finish the work on this feature. If you want to modify the code, please enable this symbol. |
| `OBSOLETE`                   | `false` | Indicates the code block is no longer in use. You can still enable to compile them, but unstable. |

