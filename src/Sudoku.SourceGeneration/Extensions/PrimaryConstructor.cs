namespace Sudoku.SourceGeneration;

/// <summary>
/// Provides with primary constructor operations.
/// </summary>
internal static class PrimaryConstructor
{
	/// <summary>
	/// Try to get corresponding member names.
	/// </summary>
	/// <typeparam name="TExtraData">The type of projected result after argument <paramref name="extraDataSelector"/> handled.</typeparam>
	/// <param name="this">The type symbol.</param>
	/// <param name="model">The semantic model instance.</param>
	/// <param name="parameterList">The parameter list.</param>
	/// <param name="primaryConstructorParameterAttributeSymbol">The primary constructor parameter type symbol.</param>
	/// <param name="attributeMatcher">Indicates the primary constructor attribute matcher.</param>
	/// <param name="extraDataSelector">Extra data selector.</param>
	/// <param name="cancellationToken">The cancellation token that can cancel the operation.</param>
	/// <returns>A list of values (member names and extra data).</returns>
	public static (string Name, TExtraData ExtraData)[] GetCorrespondingMemberNames<TExtraData>(
		this INamedTypeSymbol @this,
		SemanticModel model,
		ParameterListSyntax? parameterList,
		INamedTypeSymbol primaryConstructorParameterAttributeSymbol,
		Func<AttributeData, bool> attributeMatcher,
		Func<ISymbol, TExtraData> extraDataSelector,
		CancellationToken cancellationToken
	)
	{
		const string Property = nameof(Property), Field = nameof(Field);
		var members = @this.GetAllMembers().ToArray();
		var baseMembers =
			from member in members
			where member is IFieldSymbol or IPropertySymbol && member.GetAttributes().Any(attributeMatcher)
			select (member.Name, extraDataSelector(member));
		return parameterList is null
			? baseMembers.ToArray()
			: (
				from parameter in parameterList.Parameters
				select model.GetDeclaredSymbol(parameter, cancellationToken) into parameterSymbol
				where !parameterSymbol.Type.IsRefLikeType // Ref structs cannot participate in the hashing.
				let attributesData = parameterSymbol.GetAttributes()
				where attributesData.Any(attributeMatcher)
				let primaryConstructorParameterAttributeData = attributesData.FirstOrDefault(primaryConstructorParameterAttributeMatcher)
				where primaryConstructorParameterAttributeData is { ConstructorArguments: [{ Value: string }] }
				let parameterKind = (string)primaryConstructorParameterAttributeData.ConstructorArguments[0].Value!
				where parameterKind is Property or Field
				let memberConversion = parameterKind switch { Property => ">@", Field => "_<@", _ => "@" }
				let namedArguments = primaryConstructorParameterAttributeData.NamedArguments
				let parameterName = parameterSymbol.Name
				let referencedMemberName = GetTargetMemberName(namedArguments, parameterName, memberConversion)
				select (referencedMemberName, extraDataSelector(parameterSymbol))
			).Concat(baseMembers).ToArray();


		bool primaryConstructorParameterAttributeMatcher(AttributeData a)
			=> SymbolEqualityComparer.Default.Equals(a.AttributeClass, primaryConstructorParameterAttributeSymbol);
	}

	public static string GetTargetMemberName(NamedArgs namedArgs, string parameterName, string defaultPattern)
		=> namedArgs.TryGetValueOrDefault<string>("GeneratedMemberName", out var customizedFieldName)
		&& customizedFieldName is not null
			? customizedFieldName
			: namedArgs.TryGetValueOrDefault<string>("NamingRule", out var namingRule) && namingRule is not null
				? namingRule.InternalHandle(parameterName)
				: defaultPattern.InternalHandle(parameterName);
}

/// <include file='../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="extension"]'/>
file static class Extensions
{
	/// <summary>
	/// Internal handle the naming rule, converting it into a valid identifier via specified parameter name.
	/// </summary>
	/// <param name="this">The naming rule.</param>
	/// <param name="parameterName">The parameter name.</param>
	/// <returns>The final identifier.</returns>
	public static string InternalHandle(this string @this, string parameterName)
		=> @this
			.Replace("<@", parameterName.ToCamelCasing())
			.Replace(">@", parameterName.ToPascalCasing())
			.Replace("@", parameterName);
}
