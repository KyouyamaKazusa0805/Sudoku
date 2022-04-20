namespace System.CommandLine.Annotations;

/// <summary>
/// Represents an attribute that is applied to a property, indicating the property value should be converted
/// through a <see cref="IValueConverter"/>.
/// </summary>
/// <seealso cref="IValueConverter"/>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class CommandConverterAttribute : CommandLineAttributeBase
{
	/// <summary>
	/// Initializes a <see cref="CommandConverterAttribute"/> instance.
	/// </summary>
	/// <param name="converterType">The converter type.</param>
	public CommandConverterAttribute(Type converterType) => ConverterType = converterType;


	/// <summary>
	/// Indicates the value converter type that allows the conversion
	/// from <see cref="string"/> to the target type.
	/// </summary>
	public Type ConverterType { get; }
}
