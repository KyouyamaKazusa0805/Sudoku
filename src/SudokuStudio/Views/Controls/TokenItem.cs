// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Markup;
using SudokuStudio.ComponentModel;
using SudokuStudio.Interaction;

namespace SudokuStudio.Views.Controls;

/// <summary>
/// Represents for a token item.
/// </summary>
[ContentProperty(Name = nameof(Content))]
[TemplatePart(Name = TokenItemRemoveButtonName, Type = typeof(ButtonBase))]
[DependencyProperty<bool>("IsRemoveable", DocSummary = "Gets or sets a value indicating whether the tab can be closed by the user with the close button.")]
[DependencyProperty<IconElement>("Icon", DocSummary = "Gets or sets the icon.")]
public partial class TokenItem : ListViewItem
{
	internal const string IconLeftState = "IconLeft";
	internal const string IconOnlyState = "IconOnly";
	internal const string ContentOnlyState = "ContentOnly";
	internal const string RemoveButtonVisibleState = "RemoveButtonVisible";
	internal const string RemoveButtonNotVisibleState = "RemoveButtonNotVisible";
	internal const string TokenItemRemoveButtonName = "PART_RemoveButton";
	internal ButtonBase? _tokenItemRemoveButton;


	/// <summary>
	/// Initializes a <see cref="TokenItem"/> instance.
	/// </summary>
	public TokenItem() => DefaultStyleKey = typeof(TokenItem);


	/// <summary>
	/// Fired when the Tab's close button is clicked.
	/// </summary>
	public event EventHandler<TokenItemRemovingEventArgs>? Removing;


	/// <inheritdoc/>
	protected override void OnApplyTemplate()
	{
		base.OnApplyTemplate();

		if (_tokenItemRemoveButton is not null)
		{
			_tokenItemRemoveButton.Click -= TokenItemRemoveButton_Click;
		}

		_tokenItemRemoveButton = GetTemplateChild(TokenItemRemoveButtonName) as ButtonBase;

		if (_tokenItemRemoveButton is not null)
		{
			_tokenItemRemoveButton.Click += TokenItemRemoveButton_Click;
		}

		IconChanged();
		ContentChanged();
		IsRemoveableChanged();
	}

	/// <inheritdoc/>
	protected override void OnContentChanged(object oldContent, object newContent)
	{
		base.OnContentChanged(oldContent, newContent);
		ContentChanged();
	}

	/// <summary>
	/// Callback method for changing <see cref="Icon"/> property.
	/// </summary>
	/// <param name="oldValue">The original value.</param>
	/// <param name="newValue">The new value.</param>
	protected virtual void OnIconPropertyChanged(IconElement oldValue, IconElement newValue) => IconChanged();

	/// <summary>
	/// Callback method for changing <see cref="IsRemoveable"/> property.
	/// </summary>
	/// <param name="oldValue">The original value.</param>
	/// <param name="newValue">The new value.</param>
	protected virtual void OnIsRemoveablePropertyChanged(bool oldValue, bool newValue) => IsRemoveableChanged();

	private void ContentChanged() => VisualStateManager.GoToState(this, Content is not null ? IconLeftState : IconOnlyState, true);

	private void IconChanged() => VisualStateManager.GoToState(this, Icon is not null ? IconLeftState : ContentOnlyState, true);

	private void IsRemoveableChanged()
		=> VisualStateManager.GoToState(this, IsRemoveable ? RemoveButtonVisibleState : RemoveButtonNotVisibleState, true);


	[Callback]
	private static void IsRemoveablePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> ((TokenItem)d).OnIsRemoveablePropertyChanged((bool)e.OldValue, (bool)e.NewValue);

	[Callback]
	private static void IconPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> ((TokenItem)d).OnIconPropertyChanged((IconElement)e.OldValue, (IconElement)e.NewValue);


	private void TokenItemRemoveButton_Click(object sender, RoutedEventArgs e)
	{
		if (IsRemoveable)
		{
			Removing?.Invoke(this, new(Content, this));
		}
	}
}
