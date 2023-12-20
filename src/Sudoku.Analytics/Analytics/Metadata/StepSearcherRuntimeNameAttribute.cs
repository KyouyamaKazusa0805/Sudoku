using System.Globalization;
using System.SourceGeneration;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics.Metadata;

/// <summary>
/// Represents the runtime step searcher name to be used and displayed in UI.
/// </summary>
/// <param name="resourceKey">The resource key to fetch the target name.</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed partial class StepSearcherRuntimeNameAttribute([Data] string resourceKey) : Attribute
{
	/// <summary>
	/// Indicates the internal name of the resource. If the configured resource key cannot find corresponding resource,
	/// <see langword="null"/> will be returned.
	/// </summary>
	/// <param name="cultureInfo">The culture information.</param>
	public string? GetFactName(CultureInfo? cultureInfo) => GetString(ResourceKey, cultureInfo ?? CultureInfo.CurrentUICulture);
}
