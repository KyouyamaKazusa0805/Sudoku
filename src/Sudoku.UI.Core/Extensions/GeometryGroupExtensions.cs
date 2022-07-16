namespace Microsoft.UI.Xaml.Shapes;

/// <summary>
/// Provides with extension methods on <see cref="GeometryGroup"/>.
/// </summary>
/// <seealso cref="GeometryGroup"/>
public static class GeometryGroupExtensions
{
	/// <summary>
	/// Sets the property <see cref="GeometryGroup.Children"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static GeometryGroup WithChildren(this GeometryGroup @this, params Geometry[] geometries)
	{
		var collection = new GeometryCollection();
		collection.AddRange(geometries);

		@this.Children = collection;
		return @this;
	}
}
