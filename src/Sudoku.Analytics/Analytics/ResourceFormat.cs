using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.SourceGeneration;
using Sudoku.Strings;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics;

/// <summary>
/// Represents a resource format. This type is used by <see cref="Step"/> instances to describe the technique format
/// stored in resource dictionary.
/// </summary>
/// <seealso cref="Step"/>
[InterpolatedStringHandler]
[StructLayout(LayoutKind.Auto)]
[Equals(EqualsBehavior.ThrowNotSupportedException)]
[GetHashCode(GetHashCodeBehavior.ThrowNotSupportedException)]
[ToString(ToStringBehavior.ThrowNotSupportedException)]
[EqualityOperators(EqualityOperatorsBehavior.Intelligent)]
[method: EditorBrowsable(EditorBrowsableState.Never)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
[method: DebuggerStepThrough]
public partial struct ResourceFormat([Data(DataMemberKinds.Field)] int literalLength, [Data(DataMemberKinds.Field)] int holeCount)
{
	/// <summary>
	/// The suffix of the format.
	/// </summary>
	private string? _formatSuffix;


	/// <summary>
	/// Indicates the format key. The value can be <see langword="null"/> if the step does not contain an equivalent resource key.
	/// </summary>
	public readonly string? Format => _formatSuffix is null ? null : GetString($"TechniqueFormat_{_formatSuffix}");


	/// <inheritdoc cref="DefaultInterpolatedStringHandler.AppendFormatted(string?)"/>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[DebuggerStepThrough]
	public void AppendFormatted(string formatSuffix) => _formatSuffix = formatSuffix;

	/// <summary>
	/// Get the format string for the current instance.
	/// </summary>
	/// <param name="formatArguments">The format arguments.</param>
	/// <returns>The final result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly string ToString(params string[] formatArguments)
		=> Format is not null
			? string.Format(Format, formatArguments)
			: throw new ResourceNotFoundException($"TechniqueFormat_{_formatSuffix}", typeof(ResourceFormat).Assembly);
}
