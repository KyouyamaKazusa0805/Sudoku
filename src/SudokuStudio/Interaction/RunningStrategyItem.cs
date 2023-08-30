namespace SudokuStudio.Interaction;

/// <summary>
/// Represents a type that describes for a running strategy item.
/// </summary>
/// <param name="titleKey">Indicates the title key in resource.</param>
/// <param name="value">Indicates the value.</param>
/// <param name="updater">The updater that can assign new value to the target place.</param>
public sealed partial class RunningStrategyItem(
	[DataMember(SetterExpression = "set")] string titleKey,
	[DataMember(SetterExpression = "set")] string value,
	[DataMember(SetterExpression = "set")] RunningStrategyUpdater updater
);
