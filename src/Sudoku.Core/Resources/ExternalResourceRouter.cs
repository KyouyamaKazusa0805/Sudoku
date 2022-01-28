#pragma warning disable IDE1006

namespace Sudoku.Resources;

/// <summary>
/// Defines a external resource router. 
/// </summary>
/// <param name="resourceKey">The resource key.</param>
/// <returns>The found external resource value. If not found, <see langword="null"/>.</returns>
public delegate string? ExternalResourceRouter(string resourceKey);