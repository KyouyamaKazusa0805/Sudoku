#undef NUMERIC_GENERIC_TYPE

namespace System;

/// <summary>
/// Defines an enumerator type that iterates on bits of a number of generic type.
/// </summary>
/// <typeparam name="TNumber">The type of the number that supports for iteration on bits.</typeparam>
/// <param name="value">The number to be iterated.</param>
/// <param name="bitsCount">The number of bits to be iterated.</param>
[StructLayout(LayoutKind.Auto)]
public ref partial struct GenericNumberEnumerator<TNumber>(
	[PrimaryConstructorParameter(MemberKinds.Field, IsImplicitlyReadOnly = false)] TNumber value,
	[PrimaryConstructorParameter(MemberKinds.Field)] int bitsCount
)
#if NUMERIC_GENERIC_TYPE
	where TNumber : IBitwiseOperators<TNumber, TNumber, TNumber>, INumber<TNumber>, IShiftOperators<TNumber, int, TNumber>
#else
	where TNumber :
		IAdditiveIdentity<TNumber, TNumber>,
		IBitwiseOperators<TNumber, TNumber, TNumber>,
		IEqualityOperators<TNumber, TNumber, bool>,
		IMultiplicativeIdentity<TNumber, TNumber>,
		IShiftOperators<TNumber, int, TNumber>
#endif
{
	/// <inheritdoc cref="IEnumerator{TNumber}.Current"/>
	public int Current { get; private set; } = -1;


	/// <inheritdoc cref="IEnumerator.MoveNext"/>
	public bool MoveNext()
	{
		while (++Current < _bitsCount)
		{
			if (
#if NUMERIC_GENERIC_TYPE
				(_value >> Current & TNumber.One) != TNumber.Zero
#else
				(_value >> Current & TNumber.MultiplicativeIdentity) != TNumber.AdditiveIdentity
#endif
			)
			{
				return true;
			}
		}

		return false;
	}
}
