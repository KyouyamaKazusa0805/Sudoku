namespace Microsoft.UI.Xaml;

/// <summary>
/// Provides a type that simply calls <see cref="DependencyProperty.Register(string, Type, Type, PropertyMetadata)"/>
/// and <see cref="DependencyProperty.RegisterAttached(string, Type, Type, PropertyMetadata)"/>.
/// </summary>
/// <seealso cref="DependencyProperty.Register(string, Type, Type, PropertyMetadata)"/>
/// <seealso cref="DependencyProperty.RegisterAttached(string, Type, Type, PropertyMetadata)"/>
internal static class DependencyPropertyRegistering
{
	/// <summary>
	/// Registers a <see cref="DependencyProperty"/> instance that simply calls the method
	/// <see cref="DependencyProperty.Register(string, Type, Type, PropertyMetadata)"/>.
	/// </summary>
	/// <typeparam name="TProperty">The type of the property.</typeparam>
	/// <typeparam name="TType">The containing type.</typeparam>
	/// <param name="propertyName">The property name.</param>
	/// <param name="defaultValue">The default value of the property.</param>
	/// <param name="callback">
	/// <inheritdoc cref="PropertyMetadata(object, PropertyChangedCallback)" path="/param[@name='propertyChangedCallback']"/>
	/// </param>
	/// <returns>A <see cref="DependencyProperty"/> instance.</returns>
	/// <seealso cref="DependencyProperty.Register(string, Type, Type, PropertyMetadata)"/>
	public static DependencyProperty RegisterDependency<TProperty, TType>(
		string propertyName,
		TProperty? defaultValue = default,
		Action<DependencyObject, TProperty>? callback = null
	) => callback switch
	{
		null => DependencyProperty.Register(propertyName, typeof(TProperty), typeof(TType), new(defaultValue)),
		_ => DependencyProperty.Register(
			propertyName,
			typeof(TProperty),
			typeof(TType),
			new(defaultValue, (d, e) => callback(d, (TProperty)e.NewValue))
		)
	};
}
