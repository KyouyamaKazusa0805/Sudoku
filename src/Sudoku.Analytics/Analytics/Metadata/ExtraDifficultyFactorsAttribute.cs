namespace Sudoku.Analytics.Metadata;

/// <summary>
/// Provides an attribute type that describes for a <see cref="Technique"/> instance supporting
/// for which kinds of extra difficulty rating rules.
/// The values can be viewed in <see cref="ExtraDifficultyFactorNames"/>.
/// </summary>
/// <param name="factorNames">
/// Indicates the name of rules. The values must be matched with fields in <see cref="ExtraDifficultyFactorNames"/>.
/// If invalid, the value will be skipped and don't work.
/// </param>
/// <seealso cref="Technique"/>
/// <seealso cref="ExtraDifficultyFactorNames"/>
/// <seealso cref="ExtraDifficultyFactor"/>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed partial class ExtraDifficultyFactorsAttribute([PrimaryConstructorParameter] params string[] factorNames) : Attribute;
