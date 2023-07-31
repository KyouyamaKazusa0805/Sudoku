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
	{
		return notation switch
		{
			Kind.Normal => toNormalString(value),
			Kind.CapitalOnly => toCapitalOnlyString(value),
			_ => throw new ArgumentOutOfRangeException(nameof(notation))
		};


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static string toNormalString(FishStep value)
		{
			var baseSets = HouseFormatter.Format(value.BaseSetsMask);
			var coverSets = HouseFormatter.Format(value.CoverSetsMask);
			var exofins = value switch
			{
				NormalFishStep { Fins: var f and not [] } => $"f{f} ",
				ComplexFishStep { Exofins: var f and not [] } => $"f{f} ",
				_ => string.Empty
			};
			var endofins = value is ComplexFishStep { Endofins: var e and not [] } ? $"ef{e}" : string.Empty;

			return $@"{value.Digit + 1} {baseSets}\{coverSets} {exofins}{endofins}";
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static string toCapitalOnlyString(FishStep value)
		{
			var baseSetLetters = HouseFormatter.Format(value.BaseSetsMask, FormattingMode.Simple);
			var coverSetLetters = HouseFormatter.Format(value.CoverSetsMask, FormattingMode.Simple);
			return $@"{baseSetLetters}\{coverSetLetters}";
		}
	}
}
