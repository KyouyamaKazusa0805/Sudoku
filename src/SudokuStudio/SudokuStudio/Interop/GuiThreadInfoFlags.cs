namespace SudokuStudio.Interop;

/// <inheritdoc cref="GUITHREADINFO.flags"/>
[Flags]
internal enum GuiThreadInfoFlags
{
	GUI_CARETBLINKING = 0x00000001,
	GUI_INMENUMODE = 0x00000004,
	GUI_INMOVESIZE = 0x00000002,
	GUI_POPUPMENUMODE = 0x00000010,
	GUI_SYSTEMMENUMODE = 0x00000008
}
