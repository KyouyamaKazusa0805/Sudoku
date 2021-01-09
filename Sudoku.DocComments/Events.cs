#nullable enable annotations

using System;

namespace Sudoku.DocComments
{
	/// <summary>
	/// Provides doc comments on event-delegating methods.
	/// </summary>
	public sealed class Events
	{
		/// <summary>
		/// Trigger execution when a control is clicked.
		/// </summary>
		/// <param name="sender">The control to trigger <c>Click</c> event.</param>
		/// <param name="e">The event information, whose value is specified as a parameter.</param>
		public void Click(object? sender, EventArgs e) => throw new NotImplementedException();

		/// <summary>
		/// Trigger execution when the text in a text box is changed.
		/// </summary>
		/// <param name="sender">The control to trigger <c>TextChanged</c> event.</param>
		/// <param name="e">The event information, whose value is specified as a parameter.</param>
		public void TextChanged(object? sender, EventArgs e) => throw new NotImplementedException();

		/// <summary>
		/// Trigger execution when a combo box selected to another one.
		/// </summary>
		/// <param name="sender">The control to trigger <c>SelectionChanged</c> event.</param>
		/// <param name="e">The event information, whose value is specified as a parameter.</param>
		public void SelectionChanged(object? sender, EventArgs e) => throw new NotImplementedException();

		/// <summary>
		/// Trigger execution when the size of a window is changed.
		/// </summary>
		/// <param name="sender">The control to trigger <c>SizeChanged</c> event.</param>
		/// <param name="e">The event information, whose value is specified as a parameter.</param>
		public void SizeChanged(object? sender, EventArgs e) => throw new NotImplementedException();

		/// <summary>
		/// Trigger execution when the value is changed.
		/// </summary>
		/// <param name="sender">The control to trigger <c>ValueChanged</c> event.</param>
		/// <param name="e">The event information, whose value is specified as a parameter.</param>
		public void ValueChanged(object? sender, EventArgs e) => throw new NotImplementedException();

		/// <summary>
		/// Trigger execution of this method when a splitter changed its position.
		/// </summary>
		/// <param name="sender">The control to trigger <c>DragDelta</c> event.</param>
		/// <param name="e">The event information, whose value is specified as a parameter.</param>
		public void DragDelta(object? sender, EventArgs e) => throw new NotImplementedException();

		/// <summary>
		/// Trigger execution when a context menu is opening.
		/// </summary>
		/// <param name="sender">The control to trigger <c>ContextMenuOpening</c> event.</param>
		/// <param name="e">The event information, whose value is specified as a parameter.</param>
		public void ContextMenuOpening(object? sender, EventArgs e) => throw new NotImplementedException();

		/// <summary>
		/// Trigger execution when the left button in mouse is pressed down.
		/// </summary>
		/// <param name="sender">The control to trigger <c>MouseLeftButtonDown</c> event.</param>
		/// <param name="e">The event information, whose value is specified as a parameter.</param>
		public void MouseLeftButtonDown(object? sender, EventArgs e) => throw new NotImplementedException();

		/// <summary>
		/// Trigger execution when the right button in mouse is pressed down.
		/// </summary>
		/// <param name="sender">The control to trigger <c>MouseRightButtonDown</c> event.</param>
		/// <param name="e">The event information, whose value is specified as a parameter.</param>
		public void MouseRightButtonDown(object? sender, EventArgs e) => throw new NotImplementedException();

		/// <summary>
		/// Trigger execution when the right button in mouse is pressed up.
		/// </summary>
		/// <param name="sender">The control to trigger <c>MouseRightButtonUp</c> event.</param>
		/// <param name="e">The event information, whose value is specified as a parameter.</param>
		public void MouseRightButtonUp(object? sender, EventArgs e) => throw new NotImplementedException();

		/// <summary>
		/// Trigger execution when the key is down before triggering key down event.
		/// </summary>
		/// <param name="sender">The control to trigger <c>PreviewKeyDown</c> event.</param>
		/// <param name="e">The event information, whose value is specified as a parameter.</param>
		public void PreviewKeyDown(object? sender, EventArgs e) => throw new NotImplementedException();

		/// <summary>
		/// Trigger execution when the mouse is pressed down.
		/// </summary>
		/// <param name="sender">The control to trigger <c>MouseDown</c> event.</param>
		/// <param name="e">The event information, whose value is specified as a parameter.</param>
		public void MouseDown(object? sender, EventArgs e) => throw new NotImplementedException();

		/// <summary>
		/// Trigger execution when the mouse is pressed up.
		/// </summary>
		/// <param name="sender">The control to trigger <c>MouseUp</c> event.</param>
		/// <param name="e">The event information, whose value is specified as a parameter.</param>
		public void MouseUp(object? sender, EventArgs e) => throw new NotImplementedException();

		/// <summary>
		/// Trigger execution when the mouse cursor is moved.
		/// </summary>
		/// <param name="sender">The control to trigger <c>MouseMove</c> event.</param>
		/// <param name="e">The event information, whose value is specified as a parameter.</param>
		public void MouseMove(object? sender, EventArgs e) => throw new NotImplementedException();

		/// <summary>
		/// Trigger execution before the mouse left button is down.
		/// </summary>
		/// <param name="sender">The control to trigger <c>PreviewMouseLeftButtonDown</c> event.</param>
		/// <param name="e">The event information, whose value is specified as a parameter.</param>
		public void PreviewMouseLeftButtonDown(object? sender, EventArgs e) => throw new NotImplementedException();

		/// <summary>
		/// Trigger execution when the control is dropped.
		/// </summary>
		/// <param name="sender">The control to trigger <c>Drop</c> event.</param>
		/// <param name="e">The event information, whose value is specified as a parameter.</param>
		public void Drop(object? sender, EventArgs e) => throw new NotImplementedException();
	}
}
