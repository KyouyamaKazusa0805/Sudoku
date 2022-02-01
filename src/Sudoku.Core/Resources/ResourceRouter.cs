namespace Sudoku.Resources;

/// <summary>
/// Defines a external resource router. 
/// </summary>
/// <param name="resourceKey">The resource key.</param>
/// <returns>The found external resource value. If not found, <see langword="null"/>.</returns>
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
public delegate string? ResourceRouter(string resourceKey);