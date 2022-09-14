// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// https://github.com/CommunityToolkit/WindowsCommunityToolkit/blob/fdaef4750236713bd788f4c1d6162a4ea5959242/Microsoft.Toolkit.Uwp.UI.Controls.Core/MetadataControl/MetadataItem.cs

namespace Sudoku.UI.Models;

/// <summary>
/// An item to display in <see cref="MetadataSequence"/>.
/// </summary>
/// <seealso cref="MetadataSequence"/>
public struct MetadataSequenceItem
{
	/// <summary>
	/// Gets or sets the label of the item.
	/// </summary>
	public string Label { get; set; }

	/// <summary>
	/// Gets or sets the parameter that will be provided to the <see cref="Command"/>.
	/// </summary>
	/// <seealso cref="Command"/>
	public object? CommandParameter { get; set; }

	/// <summary>
	/// Gets or sets the foreground of the text block.
	/// </summary>
	public Brush? Foreground { get; set; }

	/// <summary>
	/// Gets or sets the command associated to the item.
	/// If <see langword="null"/>, the item will be displayed as a text field.
	/// If set, the item will be displayed as an hyperlink.
	/// </summary>
	public ICommand Command { get; set; }
}
