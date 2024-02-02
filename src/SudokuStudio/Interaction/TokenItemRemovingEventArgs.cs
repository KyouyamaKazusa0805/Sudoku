// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace SudokuStudio.Interaction;

/// <summary>
/// Represents for event handler data provider.
/// </summary>
/// <param name="item">Item being removed.</param>
/// <param name="tokenItem"><see cref="TokenItem"/> container being closed.</param>
public partial class TokenItemRemovingEventArgs([PrimaryCosntructorParameter] object item, [PrimaryCosntructorParameter] TokenItem tokenItem) : EventArgs;
