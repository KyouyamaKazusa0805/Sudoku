namespace Sudoku.Resources;

/// <summary>
/// Defines a handler that is triggered when the specified key in the base resource documents cannot be found.
/// </summary>
/// <param name="manager">The manager that controls the resource document.</param>
/// <param name="key">The key to be found.</param>
/// <returns>
/// The <see cref="string"/>? result indicating the external resource value.
/// </returns>
public delegate string? ResourceDocumentKeyNotFoundEventHandler(ResourceDocumentManager manager, string key);