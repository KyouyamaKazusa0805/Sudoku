namespace Sudoku.Analytics.Construction.Components;

/// <summary>
/// Represents an attribute type that is applied to a field in type <see cref="LinkType"/>,
/// indicating the corresponding chaining rule.
/// </summary>
/// <typeparam name="TChainingRule">The type of the target chaining rule.</typeparam>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
internal sealed class ChainingRuleAttribute<TChainingRule> : Attribute where TChainingRule : ChainingRule, new();
