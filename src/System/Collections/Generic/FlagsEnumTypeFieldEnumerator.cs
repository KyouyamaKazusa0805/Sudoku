using System.Reflection;
using static System.Numerics.BitOperations;

namespace System.Collections.Generic;

/// <summary>
/// Defines an enumerator that iterates the possible fields of an enumeration type.
/// </summary>
/// <typeparam name="TEnum">
/// The type of the enumeration type, that is marked the attribute <see cref="FlagsAttribute"/>.
/// </typeparam>
public ref partial struct FlagsEnumTypeFieldEnumerator<TEnum> where TEnum : unmanaged, Enum
{
	/// <summary>
	/// Indicates the fields of the type to iterate.
	/// </summary>
	private readonly TEnum[] _fields;

	/// <summary>
	/// Indicates the base field.
	/// </summary>
	private readonly TEnum _base;

	/// <summary>
	/// Indicates the current index being iterated.
	/// </summary>
	private int _index = -1;


	/// <summary>
	/// Initializes a <see cref="FlagsEnumTypeFieldEnumerator{T}"/> instance via the type argument,
	/// and the base field.
	/// </summary>
	/// <param name="base">The base field to iterate.</param>
	/// <exception cref="InvalidOperationException">
	/// Throws when the type <typeparamref name="TEnum"/> is not marked <see cref="FlagsAttribute"/>.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal FlagsEnumTypeFieldEnumerator(TEnum @base)
	{
		if (!typeof(TEnum).IsDefined(typeof(FlagsAttribute)))
		{
			string message = $"Cannot operate because the type '{typeof(TEnum).Name}' isn't applied attribute type '{nameof(FlagsAttribute)}'.";
			throw new InvalidOperationException(message);
		}

		_base = @base;
		_fields = Enum.GetValues<TEnum>();
	}


	/// <inheritdoc cref="IEnumerator{T}.Current"/>
	public TEnum Current { get; private set; } = default;


	/// <inheritdoc cref="IEnumerator.MoveNext"/>
	public unsafe bool MoveNext()
	{
		for (int index = _index + 1, length = _fields.Length; index < length; index++)
		{
			var field = _fields[index];
			switch (sizeof(TEnum))
			{
				case 1 or 2 or 4 when IsPow2(Unsafe.As<TEnum, int>(ref field)) && _base.Flags(field):
				{
					Current = _fields[_index = index];
					return true;
				}
				case 8 when IsPow2(Unsafe.As<TEnum, long>(ref field)) && _base.Flags(field):
				{
					Current = _fields[_index = index];
					return true;
				}
			}
		}

		return false;
	}


	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly FlagsEnumTypeFieldEnumerator<TEnum> GetEnumerator() => this;
}
