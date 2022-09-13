// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// https://github.com/CommunityToolkit/WindowsCommunityToolkit/blob/fdaef4750236713bd788f4c1d6162a4ea5959242/Microsoft.Toolkit.Uwp.UI.Controls.Core/MetadataControl/MetadataControl.cs

namespace Sudoku.UI.Views.Controls;

/// <summary>
/// Defines a metadata sequence control that is like <see cref="MetadataControl"/>.
/// </summary>
/// <seealso cref="MetadataControl"/>
public sealed partial class MetadataSequence : UserControl
{
	/// <summary>
	/// The DP to store the <see cref="Separator"/> property value.
	/// </summary>
	public static readonly DependencyProperty SeparatorProperty =
		DependencyProperty.Register(
			nameof(Separator),
			typeof(string),
			typeof(MetadataControl),
			new(" • ", static (d, _) => ((MetadataSequence)d).Update())
		);

	/// <summary>
	/// The DP to store the <see cref="ItemsSource"/> property value.
	/// </summary>
	public static readonly DependencyProperty ItemsSourceProperty =
		DependencyProperty.Register(
			nameof(ItemsSource),
			typeof(IEnumerable<MetadataItem>),
			typeof(MetadataControl),
			new(
				null,
				static (d, e) =>
				{
					var control = (MetadataSequence)d;
					if (e.OldValue is INotifyCollectionChanged oldNcc)
					{
						oldNcc.CollectionChanged -= onCollectionChanged;
					}

					if (e.NewValue is INotifyCollectionChanged newNcc)
					{
						newNcc.CollectionChanged += onCollectionChanged;
					}

					control.Update();


					void onCollectionChanged(object? _, NotifyCollectionChangedEventArgs __) => control.Update();
				}
			)
		);


	/// <summary>
	/// Initializes a <see cref="MetadataSequence"/> control.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public MetadataSequence() => InitializeComponent();


	/// <summary>
	/// Gets or sets the separator to display between the <see cref="MetadataItem"/>.
	/// </summary>
	public string Separator
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (string)GetValue(SeparatorProperty);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => SetValue(SeparatorProperty, value);
	}

	/// <summary>
	/// Gets or sets the <see cref="MetadataItem"/> to display in the control.
	/// If it implements <see cref="INotifyCollectionChanged"/>, the control will automatically update itself.
	/// </summary>
	public IEnumerable<MetadataItem> ItemsSource
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (IEnumerable<MetadataItem>)GetValue(ItemsSourceProperty);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => SetValue(ItemsSourceProperty, value);
	}


	/// <summary>
	/// Inner update method.
	/// </summary>
	private void Update()
	{
		if (_cTextBlock is null)
		{
			// The template is not ready yet.
			return;
		}

		_cTextBlock.Inlines.Clear();

		if (ItemsSource is null)
		{
			notifyLiveRegionChanged();

			return;
		}

		Inline unitToAppend;
		foreach (var unit in ItemsSource)
		{
			if (_cTextBlock.Inlines.Count > 0)
			{
				_cTextBlock.Inlines.Add(new Run { Text = Separator });
			}

			unitToAppend = new Run { Text = unit.Label };

			if (unit.Command is not null)
			{
				var hyperLink = new Hyperlink { UnderlineStyle = UnderlineStyle.None, Foreground = _cTextBlock.Foreground };
				hyperLink.Inlines.Add(unitToAppend);
				hyperLink.Click += (_, _) =>
				{
					if (unit.Command.CanExecute(unit.CommandParameter))
					{
						unit.Command.Execute(unit.CommandParameter);
					}
				};

				unitToAppend = hyperLink;
			}

			_cTextBlock.Inlines.Add(unitToAppend);
		}

		notifyLiveRegionChanged();


		void notifyLiveRegionChanged()
		{
			if (AutomationPeer.ListenerExists(AutomationEvents.LiveRegionChanged))
			{
				FrameworkElementAutomationPeer.FromElement(this)?
					.RaiseAutomationEvent(AutomationEvents.LiveRegionChanged);
			}
		}
	}
}
