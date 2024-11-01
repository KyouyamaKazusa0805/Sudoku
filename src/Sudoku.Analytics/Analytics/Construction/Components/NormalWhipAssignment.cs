namespace Sudoku.Analytics.Construction.Components;

/// <summary>
/// Represents a type that is used by whip chaining, describing the target candidate, and why it can become available.
/// </summary>
/// <param name="Candidate">Indicates the target candidate.</param>
/// <param name="Reason"><inheritdoc cref="WhipAssignment.Reason" path="/summary"/></param>
public sealed record NormalWhipAssignment(Candidate Candidate, Technique Reason) :
	WhipAssignment(Candidate.AsCandidateMap(), Reason);
