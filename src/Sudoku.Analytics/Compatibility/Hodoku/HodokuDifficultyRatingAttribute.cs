using System.SourceGeneration;

namespace Sudoku.Compatibility.Hodoku;

/// <summary>
/// Defines an attribute that is applied to a field in technique, indicating difficulty rating value defined by Hodoku.
/// </summary>
/// <param name="difficultyRating">Indicates the difficulty rating.</param>
/// <param name="difficultyLevel">Indicates the difficulty level.</param>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed partial class HodokuDifficultyRatingAttribute([Data] int difficultyRating, [Data] HodokuDifficultyLevel difficultyLevel) :
	Attribute;
