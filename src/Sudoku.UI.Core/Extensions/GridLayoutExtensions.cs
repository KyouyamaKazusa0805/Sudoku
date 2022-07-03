namespace Microsoft.UI.Xaml.Controls;

/// <summary>
/// Provides with extension methods for <see cref="GridLayout"/>.
/// </summary>
/// <seealso cref="GridLayout"/>
public static class GridLayoutExtensions
{
	/// <summary>
	/// Sets the property <see cref="GridLayout.Padding"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static GridLayout WithPadding(this GridLayout @this, Thickness padding)
	{
		@this.Padding = padding;
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
