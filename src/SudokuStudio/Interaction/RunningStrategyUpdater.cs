namespace SudokuStudio.Interaction;

/// <summary>
/// Represents a type that encapsulates a router that can route a value to the distination.
/// </summary>
/// <param name="titleKey">The title key to be assigned. This value is used for disctincting with options set.</param>
/// <param name="updaterControlCreator">A function that creates a control that can update the result.</param>
/// <param name="initializedValueDisplayer">The displayer that displays for initialized values.</param>
/// <param name="valueRouter">The value router that can assign the value to the distinction.</param>
public sealed partial class RunningStrategyUpdater(
	[DataMember(SetterExpression = "set")] string titleKey,
	[DataMember(SetterExpression = "set")] Func<FrameworkElement> updaterControlCreator,
	[DataMember(SetterExpression = "set")] Func<string> initializedValueDisplayer,
	[DataMember(SetterExpression = "set")] Action<FrameworkElement> valueRouter
);
