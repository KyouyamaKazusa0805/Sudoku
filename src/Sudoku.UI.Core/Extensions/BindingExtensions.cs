namespace Microsoft.UI.Xaml.Data;

/// <summary>
/// Provides extension methods on <see cref="Binding"/>.
/// </summary>
/// <seealso cref="Binding"/>
public static class BindingExtensions
{
	/// <summary>
	/// Sets the property <see cref="Binding.ElementName"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Binding WithElementName(this Binding @this, string elementName)
	{
		@this.ElementName = elementName;
		return @this;
	}

	/// <summary>
	/// Sets the property <see cref="Binding.Path"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Binding WithPath(this Binding @this, string path) => @this.WithPath(new PropertyPath(path));

	/// <summary>
	/// Sets the property <see cref="Binding.Path"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Binding WithPath(this Binding @this, PropertyPath path)
	{
		@this.Path = path;
		return @this;
	}

	/// <summary>
	/// Sets the property <see cref="Binding.Converter"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Binding WithConverter<TValueConverter>(this Binding @this) where TValueConverter : IValueConverter, new()
	{
		@this.Converter = new TValueConverter();
		return @this;
	}

	/// <summary>
	/// Sets the property <see cref="Binding.UpdateSourceTrigger"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Binding WithUpdateSourceTrigger(this Binding @this, UpdateSourceTrigger updateSourceTrigger)
	{
		@this.UpdateSourceTrigger = updateSourceTrigger;
		return @this;
	}

	/// <summary>
	/// Sets the property <see cref="Binding.Mode"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Binding WithMode(this Binding @this, BindingMode mode)
	{
		@this.Mode = mode;
		return @this;
	}
}
