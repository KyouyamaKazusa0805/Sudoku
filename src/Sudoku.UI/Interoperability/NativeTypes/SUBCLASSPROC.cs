namespace Sudoku.UI.Interoperability.NativeTypes;

[SuppressMessage("Style", "IDE1006:Naming Style", Justification = "<Pending>")]
internal delegate int SUBCLASSPROC(nint hWnd, uint uMsg, nint wParam, nint lParam, nint uIdSubclass, uint dwRefData);