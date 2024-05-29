namespace SudokuStudio.Collection;

/// <summary>
/// Represents a color palette that contains a list of <see cref="Color"/> instances that can be used in UI binding.
/// </summary>
[Equals]
[ToString]
[EqualityOperators]
public sealed partial class ColorPalette :
	ObservableCollection<Color>,
	IEquatable<ColorPalette>,
	IEqualityOperators<ColorPalette, ColorPalette, bool>
{
	[StringMember]
	private string ElementsString => $"[{string.Join(", ", this)}]";


	/// <inheritdoc/>
	public bool Equals([NotNullWhen(true)] ColorPalette? other)
	{
		if (other is null)
		{
			return false;
		}

		if (Count != other.Count)
		{
			return false;
		}

		for (var i = 0; i < Count; i++)
		{
			if (this[i] != other[i])
			{
				return false;
			}
		}
		return true;
	}

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		var result = new HashCode();
		foreach (var element in this)
		{
			result.Add(element);
		}
		return result.ToHashCode();
	}
}
