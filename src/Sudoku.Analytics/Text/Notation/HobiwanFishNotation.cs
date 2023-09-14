using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Sudoku.Analytics.Steps;

namespace Sudoku.Text.Notation;

/// <summary>
/// Represents a type that provides with notation output for Hobiwan fishes.
/// </summary>
public sealed partial class HobiwanFishNotation : INotation<HobiwanFishNotation, FishStep, HobiwanFishNotation.Kind>
{
	/// <inheritdoc/>
	/// <exception cref="NotSupportedException">Throws always. This method will never be implemented.</exception>
	[DoesNotReturn]
	static FishStep INotation<HobiwanFishNotation, FishStep, Kind>.Parse(string text, Kind notation)
		=> throw new NotSupportedException("This operation is not supported and will never be implemented.");

	/// <inheritdoc cref="ToString(FishStep, Kind)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToString(FishStep value) => ToString(value, Kind.Normal);

	/// <inheritdoc/>
	public static string ToString(FishStep value, Kind notation)
		=> notation switch
		{
			Kind.Normal
				when HouseNotation.ToMaskString(value.BaseSetsMask) is var baseSets
				&& HouseNotation.ToMaskString(value.CoverSetsMask) is var coverSets
				&& value switch
				{
					NormalFishStep { Fins: var f and not [] } => $"f{f} ",
					ComplexFishStep { Exofins: var f and not [] } => $"f{f} ",
					_ => string.Empty
				} is var exofins
				&& (value is ComplexFishStep { Endofins: var e and not [] } ? $"ef{e}" : string.Empty) is var endofins
				=> $@"{value.Digit + 1} {baseSets}\{coverSets} {exofins}{endofins}",
			Kind.CapitalOnly
				when HouseNotation.ToMaskString(value.BaseSetsMask, HouseNotation.Kind.CapitalOnly) is var baseSetLetters
				&& HouseNotation.ToMaskString(value.CoverSetsMask, HouseNotation.Kind.CapitalOnly) is var coverSetLetters
				=> $@"{baseSetLetters}\{coverSetLetters}",
			_ => throw new ArgumentOutOfRangeException(nameof(notation))
		};
}
