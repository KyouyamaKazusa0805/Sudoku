using System.Reflection;
using System.Runtime.CompilerServices;
using System.SourceGeneration;
using static System.Numerics.BitOperations;

namespace System.Collections.Generic;

/// <summary>
/// Defines an enumerator that iterates the possible fields of an enumeration type.
/// </summary>
/// <typeparam name="T">The type of the enumeration type, that is marked the attribute <see cref="FlagsAttribute"/>.</typeparam>
[Equals]
[GetHashCode]
public ref partial struct FlagsEnumTypeFieldEnumerator<T> where T : unmanaged, Enum
{
	/// <summary>
	/// Indicates the fields of the type to iterate.
	/// </summary>
	private readonly T[] _fields;

	/// <summary>
	/// Indicates the base field.
	/// </summary>
	private readonly T _base;

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
	/// Throws when the type <typeparamref name="T"/> is not marked <see cref="FlagsAttribute"/>.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal FlagsEnumTypeFieldEnumerator(T @base)
		=> (_base, _fields) = typeof(T).IsDefined(typeof(FlagsAttribute))
			? (@base, Enum.GetValues<T>())
			: throw new InvalidOperationException($"Cannot operate because the type '{typeof(T).Name}' isn't applied attribute type '{nameof(FlagsAttribute)}'.");


	/// <inheritdoc cref="IEnumerator{T}.Current"/>
	public T Current { get; private set; } = default;


	/// <inheritdoc cref="IEnumerator.MoveNext"/>
	public unsafe bool MoveNext()
	{
		for (var index = _index + 1; index < _fields.Length; index++)
		{
			var field = _fields[index];
			switch (sizeof(T))
			{
				case 1 or 2 or 4 when IsPow2(As<T, int>(ref field)) && _base.Flags(field):
				{
					Current = _fields[_index = index];
					return true;
				}
				case 8 when IsPow2(As<T, long>(ref field)) && _base.Flags(field):
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
	public readonly FlagsEnumTypeFieldEnumerator<T> GetEnumerator() => this;
}
