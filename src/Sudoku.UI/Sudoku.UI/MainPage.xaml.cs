using MemberNotNull = System.Diagnostics.CodeAnalysis.MemberNotNullAttribute;

namespace Sudoku.UI
{
	/// <summary>
	/// The basic interactions about the <c><see cref="MainPage"/>.xaml</c>.
	/// </summary>
	public partial class MainPage
	{
		/// <summary>
		/// Initializes a <see cref="MainPage"/> instance with no parameters.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This constructor will calls the inner method called <c>InitializeComponent</c>,
		/// but this method will automatically iniatilize all controls in this page implicitly,
		/// but Roslyn don't know this behavior, so it'll report a
		/// <see href="https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/upgrade-to-nullable-references#warnings-help-discover-original-design-intent">
		/// CS8618
		/// </see>
		/// compiler warning.
		/// </para>
		/// <para>
		/// To be honest, I suggest the team will append <see cref="MemberNotNull"/>
		/// onto the method <c>InitializeComponent</c> in order to avoid Roslyn making the error wave
		/// mark on this method.
		/// </para>
		/// </remarks>
		/// <seealso cref="MemberNotNull"/>
#if NULLABLE
#nullable disable
		public MainPage() => InitializeComponent();
#nullable restore
#endif
	}
}
