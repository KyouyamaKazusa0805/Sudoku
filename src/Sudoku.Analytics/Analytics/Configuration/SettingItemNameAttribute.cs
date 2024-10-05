namespace Sudoku.Analytics.Configuration;

/// <summary>
/// Represents an attribute that can be applied to a property in a <see cref="StepSearcher"/>,
/// indicating the runtime identifier. This property will be used for checking and replacing values in runtime.
/// </summary>
/// <param name="identifier">
/// Indicates the runtime identifier value. You can use <see cref="SettingItemNames"/> type to get the target name.
/// </param>
/// <seealso cref="StepSearcher"/>
/// <seealso cref="SettingItemNames"/>
[AttributeUsage(AttributeTargets.Property, Inherited = false)]
public sealed partial class SettingItemNameAttribute([Property] string identifier) : Attribute;
