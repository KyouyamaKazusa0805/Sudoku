using System.Globalization;
using Sudoku.Concepts;

namespace Sudoku.Analytics;

/// <summary>
/// Represents a progress used by
/// <see cref="IAnalyzer{TSelf, TResult}.Analyze(ref readonly Grid, CultureInfo?, IProgress{AnalyzerProgress}?, CancellationToken)"/>.
/// </summary>
/// <param name="StepSearcherName">Indicates the currently used step searcher.</param>
/// <param name="Percent">The percent value.</param>
/// <seealso cref="IAnalyzer{TSelf, TResult}.Analyze(ref readonly Grid, CultureInfo?, IProgress{AnalyzerProgress}?, CancellationToken)"/>
public readonly record struct AnalyzerProgress(string StepSearcherName, double Percent);
