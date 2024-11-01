namespace System.Resources;

/// <summary>
/// Represents an object that can use lookup syntax <c>resource["Key"]</c> or property syntax <c>resource.Key</c> to fetch resources.
/// </summary>
/// <remarks>
/// Usages:
/// <list type="bullet">
/// <item>Indexer: <c>resource[ResourceKeyName]</c></item>
/// <item>Indexer: <c>resource[ResourceKeyName, <see cref="string"/>]</c></item>
/// <item>Indexer: <c>resource[ResourceKeyName, <see cref="string"/>, <see cref="Assembly"/>]</c></item>
/// <item>Indexer: <c>resource[ResourceKeyName, <see cref="Assembly"/>]</c></item>
/// <item>Indexer: <c>resource[ResourceKeyName, <see cref="CultureInfo"/>]</c></item>
/// <item>Indexer: <c>resource[ResourceKeyName, <see cref="CultureInfo"/>, <see cref="Assembly"/>]</c></item>
/// <item>Property: <c>resource.ResourceKeyName</c></item>
/// <item>Method: <c>resource.ResourceKeyName()</c></item>
/// <item>Method: <c>resource.ResourceKeyName(<see cref="string"/>)</c></item>
/// <item>Method: <c>resource.ResourceKeyName(<see cref="string"/>, <see cref="Assembly"/>)</c></item>
/// <item>Method: <c>resource.ResourceKeyName(<see cref="CultureInfo"/>)</c></item>
/// <item>Method: <c>resource.ResourceKeyName(<see cref="CultureInfo"/>, <see cref="Assembly"/>)</c></item>
/// </list>
/// All valid invocations will return a <see cref="string"/> result indicating result.
/// </remarks>
public sealed class DynamicResource : DynamicObject
{
	/// <summary>
	/// Represents the default instance.
	/// </summary>
	public static readonly dynamic Instance = new DynamicResource();


	/// <summary>
	/// Indicates the singleton constructor.
	/// </summary>
	private DynamicResource()
	{
	}


	/// <inheritdoc/>
	public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, [NotNullWhen(true)] out object? result)
	{
		var (flag, resourceKey, culture, assembly) = indexes switch
		{
			[string r] => (true, r, null, null),
			[string r, string s] => (true, r, new CultureInfo(s), null),
			[string r, string s, Assembly a] => (true, r, new CultureInfo(s), a),
			[string r, Assembly a] => (true, r, null, a),
			[string r, CultureInfo c] => (true, r, c, null),
			[string r, CultureInfo c, Assembly a] => (true, r, c, a),
			_ => (false, null!, null, null)
		};
		if (!flag)
		{
			goto ReturnFalse;
		}

		if (SR.TryGet(resourceKey, out var resource, culture, assembly))
		{
			result = resource;
			return true;
		}

	ReturnFalse:
		result = null;
		return false;
	}

	/// <inheritdoc/>
	public override bool TryInvokeMember(InvokeMemberBinder binder, object?[]? args, [NotNullWhen(true)] out object? result)
	{
		var (flag, culture, assembly) = args switch
		{
			null or [] => (true, null, null),
			[string s] => (true, new CultureInfo(s), null),
			[string s, Assembly a] => (true, new CultureInfo(s), a),
			[Assembly a] => (true, null, a),
			[CultureInfo c] => (true, c, null),
			[CultureInfo c, Assembly a] => (true, c, a),
			_ => (false, null, null)
		};
		if (!flag)
		{
			goto ReturnFalse;
		}

		var propertyName = binder.Name;
		if (SR.TryGet(propertyName, out var resource, culture, assembly))
		{
			result = resource;
			return true;
		}

	ReturnFalse:
		result = null;
		return false;
	}
}
