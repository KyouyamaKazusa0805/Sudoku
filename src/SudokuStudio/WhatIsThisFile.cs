/// <summary>
/// Provides with the word "Happy".
/// </summary>
internal static class Happy
{
	/// <summary>
	/// Provides with the word "New".
	/// </summary>
	internal readonly struct New
	{
		/// <summary>
		/// Provides with the word "Year".
		/// </summary>
		[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
		internal interface Year
		{
			/// <summary>
			/// To print a greeting message!
			/// </summary>
			public static sealed void PrintGreetingMessageForYou() => Console.WriteLine("Hey! This is an easter egg for you! Happy new year!");
		}
	}
}
