namespace SudokuStudio.Configuration;

/// <summary>
/// Defines a preference group that contains a list of preference items that can be serialized and deserialized by JSON.
/// </summary>
public abstract class PreferenceGroup : DependencyObject
{
	/// <summary>
	/// Try to cover the preference from the specified instance <paramref name="other"/>.
	/// </summary>
	/// <param name="other">The newer instance that is used for covering the current instance.</param>
	public void CoverBy(PreferenceGroup other)
	{
		foreach (var propertyInfo in GetType().GetProperties())
		{
			if (!propertyInfo.CanWrite)
			{
				// Cannot cover with the new value.
				continue;
			}

			if (propertyInfo.Name is nameof(Dispatcher) or nameof(DispatcherQueue))
			{
				// Special properties.
				continue;
			}

			propertyInfo.SetValue(this, propertyInfo.GetValue(other));
		}
	}

	/// <summary>
	/// Try to fetch the default value of the specified property.
	/// </summary>
	/// <typeparam name="TGroup">The group of the property.</typeparam>
	/// <typeparam name="TProperty">The target type of the property will return.</typeparam>
	/// <param name="propertyName">The name of the property.</param>
	/// <returns>The default value of the property type.</returns>
	public static TProperty? GetDefaultValueOfProperty<TGroup, TProperty>(string propertyName) where TGroup : PreferenceGroup
	{
		const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
		return typeof(TGroup) switch
		{
			// Firstly, try to use reflection to get default value property and return.
			var targetGroupTypeInfo => targetGroupTypeInfo.GetField($"{propertyName}DefaultValue", bindingFlags) switch
			{
				{ } targetFieldInfo => (TProperty?)targetFieldInfo.GetValue(null),

				// Secondly, try to get dependency property attribute and get its default value.
				_ => targetGroupTypeInfo.GetCustomGenericAttributes(typeof(DependencyPropertyAttribute<>)) switch
				{
					var attributeTypes and not [] => (
						from attributeTypeInstance in attributeTypes
						let targetTypeInfo = attributeTypeInstance.GetType()
						let nameToCheck = (string?)targetTypeInfo.GetProperty("PropertyName")!.GetValue(attributeTypeInstance)
						where nameToCheck == propertyName
						select (TProperty?)targetTypeInfo.GetProperty("DefaultValue")!.GetValue(attributeTypeInstance)
					).FirstOrDefault(),

					// If attribute type is still not found, return default(T).
					_ => default
				}
			}
		};
	}
}
