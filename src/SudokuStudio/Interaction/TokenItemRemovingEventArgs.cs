// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.SourceGeneration;
using SudokuStudio.Views.Controls;

namespace SudokuStudio.Interaction;

/// <summary>
/// Represents for event handler data provider.
/// </summary>
/// <param name="item">Item being removed.</param>
/// <param name="tokenItem"><see cref="TokenItem"/> container being closed.</param>
public partial class TokenItemRemovingEventArgs([Data] object item, [Data] TokenItem tokenItem) : EventArgs;
