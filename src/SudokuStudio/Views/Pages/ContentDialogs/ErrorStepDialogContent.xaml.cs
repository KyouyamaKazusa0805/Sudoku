namespace SudokuStudio.Views.Pages.ContentDialogs;

/// <summary>
/// Defines a content dialog content that describes for an error step.
/// </summary>
public sealed partial class ErrorStepDialogContent : Page
{
	/// <summary>
	/// Initializes an <see cref="ErrorStepDialogContent"/> instance.
	/// </summary>
	public ErrorStepDialogContent() => InitializeComponent();


	/// <summary>
	/// <para>Indicates the error step text.</para>
	/// <para><i>
	/// This property is <see langword="set"/>-only, which means you can only assign the property with a new value.
	/// </i></para>
	/// </summary>
	/// <value>
	/// <para><inheritdoc cref="ErrorStepText" path="//summary/para[1]"/></para>
	/// <para>
	/// This value can be get by calling <see cref="object.ToString"/> method of type <see cref="Step"/> like:
	/// <code>
	/// ErrorStepText = wrongStep.ToString();
	/// </code>
	/// </para>
	/// </value>
	/// <seealso cref="Step"/>
	public string ErrorStepText { set => ErrorStepDetailsDisplayer.Text = value; }

	/// <summary>
	/// <para>Indicates the error step grid.</para>
	/// <para><inheritdoc cref="ErrorStepText" path="//summary/para[2]"/></para>
	/// </summary>
	/// <value><inheritdoc cref="ErrorStepGrid" path="//summary/para[1]"/></value>
	public Grid ErrorStepGrid
	{
		set
		{
			ErrorStepDisplayer.Puzzle = value;
			ErrorStepDisplayer.ViewUnit = null;
		}
	}

	/// <summary>
	/// <para>Indicates the <see cref="ViewNode"/>s displayed for the current wrong step.</para>
	/// <para><inheritdoc cref="ErrorStepText" path="//summary/para[2]"/></para>
	/// </summary>
	/// <value>The value of the view.</value>
	public ViewUnitBindableSource ViewUnit { set => ErrorStepDisplayer.ViewUnit = value; }
}
