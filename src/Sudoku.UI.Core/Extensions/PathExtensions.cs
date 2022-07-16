namespace Microsoft.UI.Xaml.Shapes;

/// <summary>
/// Provides with extension methods on <see cref="Path"/>.
/// </summary>
/// <seealso cref="Path"/>
public static class PathExtensions
{
	/// <summary>
	/// Sets the property <see cref="Path.Data"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Path WithData<TGeometry>(this Path @this, TGeometry geometry) where TGeometry : Geometry
	{
		@this.Data = geometry;
		return @this;
	}
}
