using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Sudoku.DocComments;

namespace System.Text
{
	/// <summary>
	/// Encapsulates a string builder implementation that is used via a <see langword="struct"/>.
	/// </summary>
	/// <remarks>
	/// You shouldn't use the parameterless constructor <see cref="ValueStringBuilder()"/>.
	/// </remarks>
	/// <example>
	/// You can use this struct like this:
	/// <code>
	/// var sb = new ValueStringBuilder(stackalloc char[100]);
	/// 
	/// // Appending operations...
	/// sb.Append("Hello");
	/// sb.Append(',');
	/// sb.Append("World");
	/// sb.Append('!');
	/// 
	/// Console.WriteLine(sb.ToString()); // Dispose method will be called here.
	/// </code>
	/// </example>
	/// <seealso cref="ValueStringBuilder()"/>
	[CLSCompliant(false)]
	[DisableParameterlessConstructor]
	public ref partial struct ValueStringBuilder
	{
		/// <summary>
		/// Indicates the inner character series that is created by <see cref="ArrayPool{T}"/>.
		/// </summary>
		/// <seealso cref="ArrayPool{T}"/>
		private char[]? _chunk;

		/// <summary>
		/// INdicates the character pool.
		/// </summary>
		private Span<char> _chars;


		/// <summary>
		/// Initializes an instance with the specified string as the basic buffer.
		/// </summary>
		/// <param name="s">The string value.</param>
		/// <remarks>
		/// This constructor should be used when you know the maximum length of the return string. In addition,
		/// the string shouldn't be too long; below 300 (approximately) is okay.
		/// </remarks>
		public unsafe ValueStringBuilder(string s)
		{
			_chunk = null;
			Length = s.Length;

			fixed (char* p = s)
			{
				_chars = new Span<char>(p, s.Length);
			}
		}

		/// <summary>
		/// Initializes an instance with the buffer specified as a <see cref="Span{T}"/>.
		/// </summary>
		/// <param name="buffer">The initial buffer.</param>
		/// <remarks>
		/// <para>
		/// For the buffer, you can use the nested <see langword="stackalloc"/> expression to create
		/// a serial of buffer, such as <c>stackalloc char[10]</c>, where the length 10 is the value
		/// that holds the approximate maximum number of characters when output from your evaluation.
		/// </para>
		/// <para>
		/// You can also use the constructor: <see cref="ValueStringBuilder(int)"/> like:
		/// <code>
		/// var sb = new ValueStringBuilder(10);
		/// </code>
		/// The code is nearly equivalent to
		/// <code>
		/// var sb = new ValueStringBuilder(stackalloc char[10]);
		/// </code>
		/// but uses shared array pool (i.e. the property <see cref="ArrayPool{T}.Shared"/>)
		/// to create the buffer rather than using <see cref="Span{T}"/>.
		/// </para>
		/// </remarks>
		/// <seealso cref="Span{T}"/>
		/// <seealso cref="ValueStringBuilder(int)"/>
		public ValueStringBuilder(in Span<char> buffer) : this() => _chars = buffer;

		/// <summary>
		/// Initializes an instance with the specified capacity.
		/// </summary>
		/// <param name="capacity">The capacity.</param>
		public ValueStringBuilder(int capacity)
		{
			_chunk = ArrayPool<char>.Shared.Rent(capacity);
			_chars = _chunk;
			Length = 0;
		}


		/// <summary>
		/// Indicates the length of the string builder.
		/// </summary>
		public int Length { get; private set; }

		/// <summary>
		/// Indicates the total capacity.
		/// </summary>
		public int Capacity => _chars.Length;


		/// <summary>
		/// Gets the reference of a character at the specified index.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns>The reference of the character.</returns>
		/// <remarks>
		/// This property returns a <see langword="ref"/> <see cref="char"/>, which
		/// means you can use the return value to re-assign a new value.
		/// </remarks>
		public ref char this[int index] => ref _chars[index];


		/// <summary>
		/// Determines whether the current instance has same values with the other instance.
		/// </summary>
		/// <param name="other">The other instance.</param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		public readonly unsafe bool Equals(in ValueStringBuilder other)
		{
			if (Length != other.Length)
			{
				return false;
			}

			fixed (char* pThis = _chars, pOther = other._chars)
			{
				int i = 0;
				for (char* p = pThis, q = pOther; i < Length; i++)
				{
					if (*p++ != *q++)
					{
						return false;
					}
				}
			}

			return true;
		}

		/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
		public readonly Enumerator GetEnumerator() => new(this);

		/// <summary>
		/// Returns the string result that is combined and constructed from the current instance.
		/// </summary>
		/// <returns>The string representation.</returns>
		/// <remarks>
		/// <para>
		/// The dispose method will be called during this method executed.
		/// Therefore, in C# 8, even if we can use the syntax
		/// <code>
		/// using var sb = new ValueStringBuilder(stackalloc char[10]);
		/// </code>
		/// we won't use the keyword <see langword="using"/> before object creation expression.
		/// </para>
		/// <para>
		/// So, you can't or don't need to:
		/// <list type="bullet">
		/// <item>Use the keyword <see langword="using"/> before object creation expression.</item>
		/// <item>Use the instance of this type after called this method.</item>
		/// </list>
		/// </para>
		/// </remarks>
		public override string ToString()
		{
			try
			{
				return _chars[..Length].ToString();
			}
			finally
			{
				Dispose();
			}
		}


		/// <inheritdoc cref="Operators.operator =="/>
		public static bool operator ==(ValueStringBuilder left, ValueStringBuilder right) => left.Equals(right);

		/// <inheritdoc cref="Operators.operator !="/>
		public static bool operator !=(ValueStringBuilder left, ValueStringBuilder right) => !(left == right);
	}
}
