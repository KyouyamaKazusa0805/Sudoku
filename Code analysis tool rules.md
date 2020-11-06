# Code Analysis Tool Rules

## Description

Sometime I found that some little bugs are too hard to find, so now I'm going to write a new code analysis tool to help me to complete the whole sudoku solution.

## Rules

| Label     | Type       | Description                                                  | Finished |
| --------- | ---------- | ------------------------------------------------------------ | -------- |
| SUDOKU001 | warning    | Missing value on `Sudoku.Windows.Resource` resource dictionary. |          |
| SUDOKU002 | suggestion | Chinese characters aren't friendly in the coding UI (may be unreadable), please extract them to `Sudoku.Windows.Resource` resource dictionary to create a new field. |          |
| SUDOKU003 | suggestion | Redundant switch checking on closed `enum`s if they are marked `'/*closed*/'`. |          |
| SUDOKU004 | warning    | Unnecessary pattern matching on basic types.                 |          |
| SUDOKU005 | warning    | `in` parameters cannot call non-`readonly` instance members because they may cause side effect to create a new copy. |          |
| SUDOKU006 | suggestion | Type implemented `IDisposable` shouldn't be initialized without `using`. |          |
| SUDOKU007 | suggestion | Avoid using interpolated strings, and replace them with `new StringBuilder().Append` pattern. |          |

