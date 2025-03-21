namespace System.CommandLine;

/// <summary>
/// Provides with property name allowed to be assigned.
/// </summary>
/// <param name="names">The names.</param>
[AttributeUsage(AttributeTargets.Property, Inherited = false)]
public sealed partial class OptionNamesAttribute([Property] params string[] names) : Attribute;
