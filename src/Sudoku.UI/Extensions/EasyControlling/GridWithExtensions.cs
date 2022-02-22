namespace Microsoft.UI.Xaml.Controls;

/// <summary>
/// Provides extension methods on <see cref="GridLayout"/>.
/// </summary>
/// <seealso cref="GridLayout"/>
internal static class GridWithExtensions
{
	/// <summary>
	/// Sets the info on <see cref="GridLayout"/>, and returns the reference
	/// of the argument <paramref name="this"/>.
	/// </summary>
	/// <typeparam name="TFrameworkElement">The type of the control.</typeparam>
	/// <param name="this">The <typeparamref name="TFrameworkElement"/>-typed control.</param>
	/// <param name="row">
	/// The row value that is used for <see cref="GridLayout.SetRow(FrameworkElement, int)"/>.
	/// </param>
	/// <param name="column">
	/// The row value that is used for <see cref="GridLayout.SetColumn(FrameworkElement, int)"/>.
	/// </param>
	/// <param name="rowSpan">
	/// The row value that is used for <see cref="GridLayout.SetRowSpan(FrameworkElement, int)"/>.
	/// </param>
	/// <param name="columnSpan">
	/// The row value that is used for <see cref="GridLayout.SetColumnSpan(FrameworkElement, int)"/>.
	/// </param>
	/// <returns>The reference that is same as the argument <paramref name="this"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFrameworkElement WithGridLayout<TFrameworkElement>(
		this TFrameworkElement @this, int row = 0, int column = 0, int rowSpan = 1, int columnSpan = 1)
		where TFrameworkElement : FrameworkElement
	{
		GridLayout.SetRow(@this, row);
		GridLayout.SetColumn(@this, column);
		GridLayout.SetRowSpan(@this, rowSpan);
		GridLayout.SetColumnSpan(@this, columnSpan);
		return @this;
	}


	/// <summary>
	/// Creates the specified number of new <see cref="RowDefinition"/> instances and adds into the target
	/// <see cref="GridLayout"/> instance.
	/// </summary>
	/// <param name="this">The <see cref="GridLayout"/> instance.</param>
	/// <param name="count">The number of <see cref="RowDefinition"/>s to be added.</param>
	/// <returns>The reference that is same as the argument <paramref name="this"/>.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="count"/> is negative.
	/// </exception>
	public static GridLayout WithRowDefinitionsCount(this GridLayout @this, int count)
	{
		switch (count)
		{
			case < 0:
			{
				throw new ArgumentOutOfRangeException(nameof(count));
			}
			case 0:
			{
				break;
			}
			default:
			{
				for (int i = 0; i < count; i++)
				{
					@this.RowDefinitions.Add(new());
				}

				break;
			}
		}

		return @this;
	}

	/// <summary>
	/// Creates the specified number of new <see cref="ColumnDefinition"/> instances and adds into the target
	/// <see cref="GridLayout"/> instance.
	/// </summary>
	/// <param name="this">The <see cref="GridLayout"/> instance.</param>
	/// <param name="count">The number of <see cref="ColumnDefinition"/>s to be added.</param>
	/// <returns>The reference that is same as the argument <paramref name="this"/>.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="count"/> is negative.
	/// </exception>
	public static GridLayout WithColumnDefinitionsCount(this GridLayout @this, int count)
	{
		switch (count)
		{
			case < 0:
			{
				throw new ArgumentOutOfRangeException(nameof(count));
			}
			case 0:
			{
				break;
			}
			default:
			{
				for (int i = 0; i < count; i++)
				{
					@this.ColumnDefinitions.Add(new());
				}

				break;
			}
		}

		return @this;
	}
}
