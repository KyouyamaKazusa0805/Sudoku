namespace Sudoku.Traits;

/// <summary>
/// Represents a direct trait.
/// </summary>
/// <typeparam name="TBase">The type of the base value.</typeparam>
public interface IDirectTrait<TBase> : ITrait where TBase : allows ref struct;
