namespace System.CommandLine.Annotations;

/// <summary>
/// Represents an attribute that is applied to a property, indicating the property value should be converted
/// through a <see cref="IValueConverter"/>.
/// </summary>
/// <typeparam name="TConverter">
/// The type of the converter. The type must implement interface type <see cref="IValueConverter"/>,
/// and contains a parameterless constructor.
/// </typeparam>
/// <seealso cref="IValueConverter"/>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class CommandConverterAttribute<TConverter> : Attribute where TConverter : class, IValueConverter, new()
{
}
