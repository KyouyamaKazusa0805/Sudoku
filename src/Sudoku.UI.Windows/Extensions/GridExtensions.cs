namespace Microsoft.UI.Xaml.Controls;

/// <summary>
/// Provides with the extension methods on <see cref="Grid"/>.
/// </summary>
/// <seealso cref="Grid"/>
public static class GridExtensions
{
	/// <summary>
	/// Adds the specified number of <see cref="RowDefinition"/>s into the <see cref="RowDefinitionCollection"/>
	/// as the property <see cref="Grid.RowDefinitions"/>.
	/// </summary>
	/// <param name="this">The current control.</param>
	/// <param name="rowDefinitionsCount">The desired number of row definitions to add.</param>
	/// <remarks>
	/// Please note that the <see cref="RowDefinition"/> instance is the default case to be added.
	/// </remarks>
	/// <seealso cref="RowDefinition"/>
	/// <seealso cref="RowDefinitionCollection"/>
	/// <seealso cref="Grid.RowDefinitions"/>
	public static void AddRowsCount(this Grid @this, int rowDefinitionsCount)
	{
		for (int i = 0; i < rowDefinitionsCount; i++)
		{
			@this.RowDefinitions.Add(new());
		}
	}

	/// <summary>
	/// Adds the specified number of <see cref="ColumnDefinition"/>s
	/// into the <see cref="ColumnDefinitionCollection"/> as the property <see cref="Grid.ColumnDefinitions"/>.
	/// </summary>
	/// <param name="this">The current control.</param>
	/// <param name="columnDefinitionsCount">The desired number of column definitions to add.</param>
	/// <remarks>
	/// Please note that the <see cref="ColumnDefinition"/> instance is the default case to be added.
	/// </remarks>
	/// <seealso cref="ColumnDefinition"/>
	/// <seealso cref="ColumnDefinitionCollection"/>
	/// <seealso cref="Grid.ColumnDefinitions"/>
	public static void AddColumnsCount(this Grid @this, int columnDefinitionsCount)
	{
		for (int i = 0; i < columnDefinitionsCount; i++)
		{
			@this.ColumnDefinitions.Add(new());
		}
	}
}
