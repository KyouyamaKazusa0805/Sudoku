using System.SourceGeneration;

namespace SudokuStudio.Interaction;

/// <summary>
/// Represents a type that describes for a running strategy item.
/// </summary>
/// <param name="titleKey">Indicates the title key in resource.</param>
/// <param name="updater">The updater that can assign new value to the target place.</param>
public sealed partial class RunningStrategyItem([Data(SetterExpression = "set")] string titleKey, [Data(SetterExpression = "set")] RunningStrategyUpdater updater);
