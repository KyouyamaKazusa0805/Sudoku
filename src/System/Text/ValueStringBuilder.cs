using System.Buffers;
using Sudoku.CodeGen;

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
	[AutoGetEnumerator("@", MemberConversion = "new(@)", ReturnType = typeof(Enumerator))]
	public ref partial struct ValueStringBuilder
	{
		/// <summary>
		/// Indicates the inner character series that is created by <see cref="ArrayPool{T}"/>.
		/// </summary>
		/// <seealso cref="ArrayPool{T}"/>
		private char[]? _chunk;

		/// <summary>
		/// Indicates the character pool.
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
		/// means you can use the return value to re-assign a new value, as the same behavior
		/// as the <see langword="set"/> accessor.
		/// </remarks>
		public ref char this[int index] => ref _chars[index];


		/// <summary>
		/// Determines whether two instances has same values with the other instance.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		[ProxyEquality]
		public static unsafe bool Equals(in ValueStringBuilder left, in ValueStringBuilder right)
		{
			if (left.Length != right.Length)
			{
				return false;
			}

			fixed (char* pThis = left._chars, pOther = right._chars)
			{
				int i = 0;
				for (char* p = pThis, q = pOther; i < left.Length; i++)
				{
					if (*p++ != *q++)
					{
						return false;
					}
				}
			}

			return true;
		}

		/// <summary>
		/// Returns the string result that is combined and constructed from the current instance,
		/// and then dispose the instance.
		/// </summary>
		/// <returns>The string representation.</returns>
		/// <remarks>
		/// <para>
		/// Please note that the dispose method will be invoked in the execution of this method <c>ToString</c>.
		/// Therefore, you can't or don't need to:
		/// <list type="bullet">
		/// <item>
		/// Use the keyword <see langword="using"/> onto the variable declaration, such as
		/// <c>using var sb = new ValueStringBuilder(stackalloc char[20]);</c>
		/// </item>
		/// <item>
		/// Use the current string builder instance after had been called this method <c>ToString</c>.
		/// </item>
		/// </list>
		/// </para>
		/// <para>Because of the such behavior, this method isn't <see langword="readonly"/>.</para>
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
	}
}
