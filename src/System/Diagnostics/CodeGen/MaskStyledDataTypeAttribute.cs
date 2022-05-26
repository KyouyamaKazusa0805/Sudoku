namespace System.Diagnostics.CodeGen;

/// <summary>
/// Provides with an attribute that is applied to a type, which contains an only field of type
/// <typeparamref name="TUnmanaged"/>, indicating the mask of the data structure.
/// </summary>
/// <typeparam name="TUnmanaged">The type of the only field.</typeparam>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public sealed class MaskStyledDataTypeAttribute<TUnmanaged> : SourceGeneratorOptionProviderAttribute
	where TUnmanaged : unmanaged
{
	/// <summary>
	/// Initializes a <see cref="MaskStyledDataTypeAttribute{TUnmanaged}"/> instance via the specified list of positions.
	/// </summary>
	/// <param name="nameAndPositions">An array of names and their own corresponding position indices.</param>
	/// <exception cref="ArgumentException">Throws when the argument has invalid values.</exception>
	public MaskStyledDataTypeAttribute(params object[] nameAndPositions)
	{
		var finalList = new List<(string, Type, int, int)>();
		for (int i = 0, last = 0; i < nameAndPositions.Length; i += 3)
		{
			if (nameAndPositions[i] is string s
				&& i + 1 < nameAndPositions.Length && nameAndPositions[i + 1] is int endIndex
				&& i + 2 < nameAndPositions.Length && nameAndPositions[i + 2] is Type type)
			{
				finalList.Add((s, type, last, endIndex));
				last = endIndex;

				continue;
			}

			throw new ArgumentException("The argument has invalid values.");
		}

		Positions = finalList.ToArray();
	}


	/// <summary>
	/// Indicates the positions that marks the start and end index separating the mask on bits.
	/// </summary>
	public (string PropertyName, Type Type, int BitIndexFrom, int BitIndexTo)[] Positions { get; }
}
