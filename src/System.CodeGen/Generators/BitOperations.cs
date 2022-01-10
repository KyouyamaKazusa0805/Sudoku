namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Indicates the generator that generates the code about extended methods of type <c>BitOperations</c>.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class BitOperations : ISourceGenerator
{
	/// <summary>
	/// Indicates the leading text.
	/// </summary>
	private const string LeadingText = @"#pragma warning disable CS1591

using static System.Numerics.BitOperations;

#nullable enable

namespace System.Numerics;

partial class BitOperationsExensions
{
	";


	private static readonly string[]
		GetAllSetsTypes = new[] { "sbyte", "byte", "short", "ushort", "int", "uint", "long", "ulong" },
		GetEnumeratorTypes = new[] { "sbyte", "byte", "short", "ushort", "int", "uint", "long", "ulong" },
		SetAtTypes = new[] { "byte", "short", "int", "long" },
		SkipSetBitTypes = new[] { "byte", "short", "int", "long" };

	private static readonly (string TypeName, int Size)[]
		GetNextSetTypes = new[] { ("byte", 8), ("short", 16), ("int", 32), ("long", 64) },
		ReverseBitsTypes = new[] { ("byte", 4), ("short", 8), ("int", 16), ("long", 32) };


	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		const string separator = "\r\n\r\n\t";
		const string typeName = "System.Numerics.BitOperationsExensions";

		context.AddSource($"{typeName}.g.cs", G_GlobalFile());

		var sb = new StringBuilder();
		(new Action(a) + b + c + d + e + f)();

		void a()
		{
			string code = string.Join(separator, from name in GetAllSetsTypes select G_GetAllSets(name));
			context.AddSource(
				typeName,
				GeneratedFileShortcuts.BitOperations_GetAllSets,
				$"{LeadingText}{code}\r\n}}\r\n"
			);
		}

		void b()
		{
			string code = string.Join(separator, from name in GetEnumeratorTypes select G_GetEnumerator(name));
			context.AddSource(
				typeName,
				GeneratedFileShortcuts.BitOperations_GetEnumerator,
				$"{LeadingText}{code}\r\n}}\r\n"
			);
		}

		void c()
		{
			string code = string.Join(separator, from pair in GetNextSetTypes select G_GetNextSet(pair.TypeName, pair.Size));
			context.AddSource(
				typeName,
				GeneratedFileShortcuts.BitOperations_GetNextSet,
				$"{LeadingText}{code}\r\n}}\r\n"
			);
		}

		void d()
		{
			string code = string.Join(separator, from pair in ReverseBitsTypes select G_ReverseBits(pair.TypeName, pair.Size));
			context.AddSource(
				typeName,
				GeneratedFileShortcuts.BitOperations_ReverseBits,
				$"{LeadingText}{code}\r\n}}\r\n"
			);
		}

		void e()
		{
			string code = string.Join(separator, from name in SetAtTypes select G_SetAt(name));
			context.AddSource(
				typeName,
				GeneratedFileShortcuts.BitOperations_SetAt,
				$"{LeadingText}{code}\r\n}}\r\n"
			);
		}

		void f()
		{
			string code = string.Join(separator, from name in SkipSetBitTypes select G_SkipSetBit(name));
			context.AddSource(
				typeName,
				GeneratedFileShortcuts.BitOperations_SkipSetBit,
				$"{LeadingText}{code}\r\n}}\r\n"
			);
		}
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context)
	{
	}


	/// <summary>
	/// Generates the global file.
	/// </summary>
	/// <returns>The string text of the code.</returns>
	private string G_GlobalFile()
	{
		var sb = new StringBuilder();
		sb.AppendLine($@"#pragma warning disable CS1591

namespace System.Numerics;

/// <summary>
/// Provides extension methods on <see cref=""BitOperations""/>.
/// </summary>
/// <seealso cref=""BitOperations""/>
[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
[global::System.Runtime.CompilerServices.CompilerGenerated]
public static partial class BitOperationsExensions
{{"
		);

		foreach (string name in GetAllSetsTypes)
			sb.AppendLine($"\tpublic static partial ReadOnlySpan<int> GetAllSets(this {name} @this);");
		sb.AppendLine();
		foreach (string name in GetEnumeratorTypes)
			sb.AppendLine($"\tpublic static partial ReadOnlySpan<int>.Enumerator GetEnumerator(this {name} @this);");
		sb.AppendLine();
		foreach (var (name, _) in GetNextSetTypes)
			sb.AppendLine($"\tpublic static partial int GetNextSet(this {name} @this, int index);");
		sb.AppendLine();
		foreach (var (name, _) in ReverseBitsTypes)
			sb.AppendLine($"\tpublic static partial void ReverseBits(this ref {name} @this);");
		sb.AppendLine();
		foreach (string name in SetAtTypes)
			sb.AppendLine($"\tpublic static partial int SetAt(this {name} @this, int order);");
		sb.AppendLine();
		foreach (string name in SkipSetBitTypes)
			sb.AppendLine($"\tpublic static partial {name} SkipSetBit(this {name} @this, int setBitPosCount);");
		return sb.Append("}\r\n").ToString();
	}

	/// <summary>
	/// Generates the file of the method <c>GetAllSets</c>.
	/// </summary>
	/// <param name="typeName">The type name.</param>
	/// <returns>The code.</returns>
	private string G_GetAllSets(string typeName)
	{
		string popCountStr = typeName switch
		{
			"uint" or "ulong" => "@this",
			"byte" or "sbyte" or "int" or "short" or "ushort" => "(uint)@this",
			"long" => "(ulong)@this"
		};

		return $@"/// <summary>
	/// Find all offsets of set bits of the binary representation of a specified value.
	/// </summary>
	/// <param name=""this"">The value.</param>
	/// <returns>All offsets.</returns>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	public static partial ReadOnlySpan<int> GetAllSets(this {typeName} @this)
	{{
		if (@this == 0)
		{{
			return ReadOnlySpan<int>.Empty;
		}}

		int length = PopCount({popCountStr});
		int[] result = new int[length];
		for (byte i = 0, p = 0; i < sizeof({typeName}) << 3; i++, @this >>= 1)
		{{
			if ((@this & 1) != 0)
			{{
				result[p++] = i;
			}}
		}}

		return result;
	}}";
	}

	/// <summary>
	/// Generates the file of the method <c>GetEnumerator</c>.
	/// </summary>
	/// <param name="typeName">The type name.</param>
	/// <returns>The code.</returns>
	private string G_GetEnumerator(string typeName) =>
		$@"/// <summary>
	/// <para>Extension get enumerator of the type <see cref=""{typeName}""/>.</para>
	/// <para>
	/// This method will allow you to use <see langword=""foreach""/> loop to iterate on
	/// all indices of set bits.
	/// </para>
	/// </summary>
	/// <param name=""this"">The value.</param>
	/// <returns>All indices of set bits.</returns>
	/// <remarks>
	/// This implementation will allow you use <see langword=""foreach""/> loop:
	/// <code><![CDATA[
	/// foreach (int setIndex in 17)
	/// {{
	/// 	// Do something...
	/// }}
	/// ]]></code>
	/// </remarks>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public static partial ReadOnlySpan<int>.Enumerator GetEnumerator(this {typeName} @this) =>
		@this.GetAllSets().GetEnumerator();";

	/// <summary>
	/// Generates the file of the method <c>GetNextSet</c>.
	/// </summary>
	/// <param name="typeName">The type name.</param>
	/// <param name="size">The size of the type.</param>
	/// <returns>The code.</returns>
	private string G_GetNextSet(string typeName, int size) =>
		$@"/// <summary>
	/// Find a index of the binary representation of a value after the specified index,
	/// whose bit is set <see langword=""true""/>.
	/// </summary>
	/// <param name=""this"">The value.</param>
	/// <param name=""index"">The index.</param>
	/// <returns>The index.</returns>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	public static partial int GetNextSet(this {typeName} @this, int index)
	{{
		for (int i = index + 1; i < {size}; i++)
		{{
			if ((@this >> i & 1) != 0)
			{{
				return i;
			}}
		}}

		return -1;
	}}";

	/// <summary>
	/// Generates the file of the method <c>ReverseBits</c>.
	/// </summary>
	/// <param name="typeName">The type name.</param>
	/// <param name="size">The size of the type.</param>
	/// <returns>The code.</returns>
	private string G_ReverseBits(string typeName, int size)
	{
		var defaults = new Dictionary<int, string[]>()
		{
			[4] = new[] { "0x55", "0x33" },
			[8] = new[] { "0x5555", "0x3333", "0x0F0F" },
			[16] = new[] { "0x55555555", "0x33333333", "0x0F0F0F0F", "0x00FF00FF" },
			[32] = new[] { "0x55555555_55555555L", "0x33333333_33333333L", "0x0F0F0F0F_0F0F0F0FL", "0x00FF00FF_00FF00FFL", "0x0000FFFF_0000FFFFL" }
		};

		var sb = new StringBuilder()
			.Append($@"/// <summary>
	/// <para>Reverse all bits in a specified value.</para>
	/// <para>
	/// Note that the value is passed by <b>reference</b> though the
	/// method is an extension method, and returns nothing.
	/// </para>
	/// </summary>
	/// <param name=""this"">The value.</param>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public static partial void ReverseBits(this ref {typeName} @this)
	{{"
			)
			.AppendLine();

		string conversion = typeName switch { "byte" => "(byte)", "short" => "(short)", _ => string.Empty };
		for (int z = 1, t = 0; z < size; z <<= 1, t++)
		{
			string q = defaults[size][t];
			sb.AppendLine($"\t\t@this = {conversion}(@this >> {z} & {q} | (@this & {q}) << {z});");
		}

		return sb
			.AppendLine($"\t\t@this = {conversion}(@this >> {size} | @this << {size});")
			.Append("\t}")
			.ToString();
	}

	/// <summary>
	/// Generates the file of the method <c>SetAt</c>.
	/// </summary>
	/// <param name="typeName">The type name.</param>
	/// <returns>The code.</returns>
	private string G_SetAt(string typeName) =>
		$@"/// <summary>
	/// Get an <see cref=""int""/> value, indicating that the absolute position of
	/// all set bits with the specified set bit order.
	/// </summary>
	/// <param name=""this"">The value.</param>
	/// <param name=""order"">The number of the order of set bits.</param>
	/// <returns>The position.</returns>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	public static partial int SetAt(this {typeName} @this, int order)
	{{
		for (int i = 0, count = -1; i < sizeof({typeName}) << 3; i++, @this >>= 1)
		{{
			if ((@this & 1) != 0 && ++count == order)
			{{
				return i;
			}}
		}}

		return -1;
	}}";

	/// <summary>
	/// Generates the file of the method <c>SkipSetBit</c>.
	/// </summary>
	/// <param name="typeName">The type name.</param>
	/// <returns>The code.</returns>
	private string G_SkipSetBit(string typeName)
	{
		string conversion = typeName switch { "byte" => "(byte)", "short" => "(short)", _ => string.Empty };
		int size = typeName switch { "byte" => 8, "short" => 16, "int" => 32, "long" => 64 };
		return $@"/// <summary>
	/// Skip the specified number of set bits and iterate on the integer with other set bits.
	/// </summary>
	/// <param name=""this"">The integer to iterate.</param>
	/// <param name=""setBitPosCount"">Indicates how many set bits you want to skip to iterate.</param>
	/// <returns>The {typeName} value that only contains the other set bits.</returns>
	/// <remarks>
	/// For example:
	/// <code><![CDATA[
	/// byte value = 0b00010111;
	/// foreach (int bitPos in value.SkipSetBit(2))
	/// {{
	///     yield return bitPos + 1;
	/// }}
	/// ]]></code>
	/// You will get 3 and 5, because all set bit positions are 0, 1, 2 and 4, and we have skipped
	/// two of them, so the result set bit positions to iterate on are only 2 and 4.
	/// </remarks>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	public static partial {typeName} SkipSetBit(this {typeName} @this, int setBitPosCount)
	{{
		{typeName} result = @this;
		for (int i = 0, count = 0; i < {size}; i++)
		{{
			if ((@this >> i & 1) != 0)
			{{
				result &= {conversion}~(1 << i);

				if (++count == setBitPosCount)
				{{
					break;
				}}
			}}
		}}

		return result;
	}}";
	}
}
