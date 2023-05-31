﻿namespace SudokuStudio.ComponentModel;

/// <summary>
/// Provides with a app-bar operation provider page.
/// </summary>
public interface IOperationProviderPage
{
	/// <summary>
	/// Indicates the base page.
	/// </summary>
	AnalyzePage BasePage { get; set; }
}
