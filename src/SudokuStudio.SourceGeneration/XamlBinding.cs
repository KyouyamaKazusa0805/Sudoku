namespace Sudoku.SourceGeneration;

/// <summary>
/// Provides with some commonly-used methods.
/// </summary>
internal static class XamlBinding
{
	public static string? GetDocumentationComment(string propertyName, DocumentationCommentData data, bool isDependencyProperty)
		=> (propertyName, data, isDependencyProperty) switch
		{
			(_, ({ } summary, { } remarks, _, _), _)
				=> $"""
				/// <summary>
						/// {summary}
						/// </summary>
						/// <remarks>
						/// {remarks}
						/// </remarks>
				""",
			(_, ({ } summary, _, _, _), _)
				=> $"""
				/// <summary>
						/// {summary}
						/// </summary>
				""",
			(_, (_, _, { } docCref, { } docPath), _)
				=> $"""
				/// <inheritdoc cref="{docCref}" path="{docPath}"/>
				""",
			(_, (_, _, { } docCref, _), _)
				=> $"""
				/// <inheritdoc cref="{docCref}"/>
				""",
			(_, _, true)
				=> $"""
				/// <summary>
						/// Indicates the interactive property that uses dependency property <see cref="{propertyName}Property"/> to get or set value.
						/// </summary>
						/// <seealso cref="{propertyName}Property" />
				""",
			(_, _, false)
				=> $"""
				/// <summary>
						/// Indicates the interactive setter or getter methods that uses attached property <see cref="{propertyName}Property"/> to get or set value.
						/// </summary>
						/// <seealso cref="{propertyName}Property" />
				"""
		};

	public static string? GetPropertyMetadataString(
		object? defaultValue,
		ITypeSymbol propertyType,
		string? generatorMemberName,
		DefaultValueGeneratingMemberKind? generatorMemberKind,
		string? callbackMethodName,
		string? propertyTypeStr
	)
	{
		const string typeName = "global::Microsoft.UI.Xaml.PropertyMetadata";
		var propertyNameFullName = propertyType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
		return (defaultValue, propertyType, generatorMemberName, generatorMemberKind, callbackMethodName) switch
		{
			(char c, _, _, _, null) => $"new {typeName}('{c}')",
			(char c, _, _, _, _) => $"new {typeName}('{c}', {callbackMethodName})",
			(string s, _, _, _, null) => $"""new {typeName}("{s}")""",
			(string s, _, _, _, _) => $"""new {typeName}("{s}", {callbackMethodName})""",
			(not null, { TypeKind: TypeKind.Enum }, _, _, null) => $"new {typeName}(({propertyNameFullName}){f()})",
			(not null, _, _, _, null) => $"new {typeName}(({propertyNameFullName}){f()})",
			(not null, { TypeKind: TypeKind.Enum }, _, _, _) => $"new {typeName}(({propertyNameFullName}){f()}, {callbackMethodName})",
			(not null, _, _, _, _) => $"new {typeName}(({propertyNameFullName}){f()}, {callbackMethodName})",
			(_, _, null, _, null) => $"new {typeName}(default({propertyTypeStr}))",
			(_, _, null, _, _) => $"new {typeName}(default({propertyTypeStr}), {callbackMethodName})",
			(_, _, not null, { } kind, _) => kind switch
			{
				DefaultValueGeneratingMemberKind.Field or DefaultValueGeneratingMemberKind.Property => callbackMethodName switch
				{
					null => $"new {typeName}({generatorMemberName})",
					_ => $"new {typeName}({generatorMemberName}, {callbackMethodName})"
				},
				DefaultValueGeneratingMemberKind.ParameterlessMethod => callbackMethodName switch
				{
					null => $"new {typeName}({generatorMemberName}())",
					_ => $"new {typeName}({generatorMemberName}(), {callbackMethodName})"
				},
				_ => null
			},
			_ => null
		};


		string f() => defaultValue.ToString().ToLower();
	}
}
