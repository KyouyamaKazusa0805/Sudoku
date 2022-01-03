namespace Nano.Diagnostics.CodeGen;

/// <summary>
/// To define an attribute that allows the source generator generating the source code
/// to notify the changes on field.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public sealed class PropertyAutoNotifyAttribute : Attribute
{
}
