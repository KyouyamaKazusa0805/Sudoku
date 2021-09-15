namespace Sudoku.Diagnostics.LanguageFeatures;

/// <summary>
/// Introduces a syntax replacer that simplifies the syntax of the full namespace declaration
/// to the file-scoped one.
/// </summary>
/// <remarks>
/// Below C# 10, the namespace should be declared with a explicit block:
/// <code>
/// namespace TestProject
/// {
///     class Program
///     {
///         static void Main()
///         {
///         }
///     }
/// }
/// </code>
/// From C# 10, we can use a comma and ignore the indentation:
/// <code>
/// namespace TestProject;
/// 
/// class Program
/// {
///     static void Main()
///     {
///     }
/// }
/// </code>
/// </remarks>
[Obsolete("The project has already upgraded to C# 10, so this type can't be used in anyway.", true)]
public sealed class FileScopedNamespaceSyntaxReplacer : ISyntaxReplacer
{
	/// <summary>
	/// Indicates the number of required match strings to replace.
	/// </summary>
	private const int RequiredStringsCount = 6;


	/// <summary>
	/// Indicates the regular expression to match the namespace declaration.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The regular expression may match four captures:
	/// <list type="table">
	/// <item>
	/// <term>$0</term>
	/// <description>The whole string. <i>The value may not be useful.</i></description>
	/// </item>
	/// <item>
	/// <term>$1</term>
	/// <description>The namespace statement line (e.g. <c>"namespace A"</c>).</description>
	/// </item>
	/// <item>
	/// <term>$2</term>
	/// <description>
	/// The last part of the whole namespace (e.g. <c>".B"</c> in <c>"A.B"</c>).
	/// <i>The value may not be useful.</i>
	/// </description>
	/// </item>
	/// <item>
	/// <term>$3</term>
	/// <description>The open curly bracket character.</description>
	/// </item>
	/// <item>
	/// <term>$4</term>
	/// <description>The other code lines. Those lines should decrease the indent.</description>
	/// </item>
	/// <item>
	/// <term>$5</term>
	/// <description>The closed curly bracket character.</description>
	/// </item>
	/// </list>
	/// </para>
	/// <para>
	/// And please note that, this regular expression instance can't process the case that the file
	/// containing multiple namespace declarations because the part <c>([\s\S]+)</c> matches all characters.
	/// </para>
	/// </remarks>
	private static readonly Regex NamespaceDeclaration = new(
		@"(?<=\s*)namespace\s+(\w+(\.\w+)*)\r\n(\{)\r\n([\s\S]+)\r\n(\})",
		RegexOptions.Compiled,
		TimeSpan.FromSeconds(5)
	);

	/// <summary>
	/// Indicates the regular expression instance to test the code to check whether the code contains
	/// the namespace declarations. If multiple exists, the code is invalid to process.
	/// </summary>
	private static readonly Regex NamespaceStatementHeader = new(
		@"(?<=\s*)namespace\s+\w+(\.\w+)*\r\n\{\r\n",
		RegexOptions.Compiled,
		TimeSpan.FromSeconds(5)
	);


	/// <summary>
	/// Initializes a <see cref="FileScopedNamespaceSyntaxReplacer"/> instance.
	/// </summary>
	public FileScopedNamespaceSyntaxReplacer()
	{
	}


	/// <inheritdoc/>
	public bool IsValid(string code)
	{
		var matches = NamespaceStatementHeader.Matches(code);
		var groups = NamespaceDeclaration.Match(code).Groups;
		return matches.Count == 1 && groups.Count == RequiredStringsCount;
	}

	/// <inheritdoc/>
	public string? Replace(string code)
	{
		string? result = IsValid(code) ? NamespaceDeclaration.Replace(code, static match =>
		{
			var groups = match.Groups;

			// For each method to invoke them.
			var sb = new StringBuilder();
			unsafe
			{
				var methods = stackalloc delegate*<StringBuilder, Group, void>[]
				{
					null,
					&processNamespaceName,
					null,
					&appendLine,
					&decreaseIndenting,
					null
				};

				for (int i = 0; i < RequiredStringsCount; i++)
				{
					if (methods[i] is var method and not null)
					{
						method(sb, groups[i]);
					}
				}
			}

			return sb.ToString();


			static void processNamespaceName(StringBuilder sb, Group group) =>
				sb.Append("namespace ").Append(group.Value).AppendLine(';');

			static void appendLine(StringBuilder sb, Group group) => sb.AppendLine();

			static void decreaseIndenting(StringBuilder sb, Group group)
			{
				string[] splits = group.Value.Split(new[] { '\r', '\n' });

				// Split operation will process both '\r's and '\n's, which means the empty string
				// will be exists in the result because the empty string may between a '\r' and a '\n'.
				// Therefore, we should increase the iteration variable by 2.
				for (int i = 0, length = splits.Length; i < length; i += 2)
				{
					string line = splits[i];
					_ = line.Length switch
					{
						> 4 when line.StartsWith("    ", StringComparison.Ordinal) => sb.AppendLine(line[4..]),
						> 1 when line.StartsWith('\t') => sb.AppendLine(line[1..]),
						0 => sb.AppendLine(),
						_ => sb.AppendLine(line)
					};
				}
			}
		}) : null;
		return result?.EndsWith("\r\n\r\n", StringComparison.Ordinal) ?? false ? result[..^2] : result;
	}
}
