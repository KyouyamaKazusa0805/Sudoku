namespace Sudoku.Maui;

/// <summary>
/// Represents an instance that can dynamically fetch the internal text resources.
/// </summary>
public sealed class TextResourceFetcher : DynamicObject
{
	/// <summary>
	/// Defines a dynamic instance that can fetch internal resource by using indexers and member invocation expression.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This instance allows you using property-reference-like expression <c>Z.PropertyName</c> or index-reference-like
	/// expression <c>Z["PropertyName"]</c> to fetch the resource.
	/// </para>
	/// <para>
	/// For instance, suppose we have a text resource declared in file <c>Resources.xaml</c>:
	/// <code><![CDATA[
	/// <x:String x:Key="HelloWorld">Hello, world!</x:String>
	/// ]]></code>
	/// If we want to fetch the value, a default way is to reference it using method invocation:
	/// <code><![CDATA[
	/// if (Application.Current.Resources.TryGetValue("HelloWorld", out var result))
	/// {
	///     // Okay.
	/// }
	/// ]]></code>
	/// This instance allows you simplify code to:
	/// <code><![CDATA[
	/// // Property-access expression.
	/// string? result = A.HelloWorld;
	/// 
	/// // Indexer-access expression.
	/// string? result = A["HelloWorld"];
	/// ]]></code>
	/// If the specified resource key is not found in target dictionary, the access will return <see langword="null"/>;
	/// otherwise, a <see langword="dynamic"/> value (actually, a <see cref="string"/>).
	/// </para>
	/// </remarks>
	public static readonly dynamic A = new TextResourceFetcher();


	/// <summary>
	/// Initializes a <see cref="TextResourceFetcher"/> instance.
	/// </summary>
	[FileAccessOnly]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private TextResourceFetcher()
	{
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool TryGetMember(GetMemberBinder binder, [NotNullWhen(true)] out object? result)
	{
		if (binder is { Name: var memberName } && ResourceTextFetcherNullable(memberName) is { } target)
		{
			result = target;
			return true;
		}

		result = null;
		return false;
	}

	/// <inheritdoc/>
	/// <param name="binder"><inheritdoc/></param>
	/// <param name="indices"><inheritdoc cref="TryGetIndex(GetIndexBinder, object[], out object?)" path="/param[@name='indexes']"/></param>
	/// <param name="result"><inheritdoc/></param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[SuppressMessage("Naming", "CA1725:Parameter names should match base declaration", Justification = "<Pending>")]
	public override bool TryGetIndex(GetIndexBinder binder, object[] indices, [NotNullWhen(true)] out object? result)
	{
		if (indices is [string key] && ResourceTextFetcherNullable(key) is { } target)
		{
			result = target;
			return true;
		}

		result = null;
		return false;
	}


	/// <summary>
	/// The internal application resource text fetcher, with <see langword="null"/> returned if not having been found.
	/// </summary>
	/// <param name="key">The key.</param>
	/// <returns>The target value fetched.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static string? ResourceTextFetcherNullable(string key)
		=> Application.Current switch
		{
			{ Resources: var resources } when resources.TryGetValue(key, out var raw) && raw is string result => result,
			_ => null
		};
}
