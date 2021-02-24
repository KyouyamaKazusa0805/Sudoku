namespace Sudoku.CodeAnalysis
{
	/// <summary>
	/// Provides diagnostic IDs.
	/// </summary>
	internal static class DiagnosticIds
	{
		public const string Sudoku001 = "SUDOKU001";
		public const string Sudoku002 = "SUDOKU002";
		public const string Sudoku003 = "SUDOKU003";
		public const string Sudoku004 = "SUDOKU004";
		public const string Sudoku005 = "SUDOKU005";
		public const string Sudoku006 = "SUDOKU006";
		public const string Sudoku007 = "SUDOKU007";
		public const string Sudoku008 = "SUDOKU008";
		public const string Sudoku009 = "SUDOKU009";
		public const string Sudoku010 = "SUDOUK010";
		public const string Sudoku011 = "SUDOKU011";
		public const string Sudoku012 = "SUDOKU012";
		public const string Sudoku013 = "SUDOKU013";
		public const string Sudoku014 = "SUDOKU014";
	}

	/// <summary>
	/// Provides help links.
	/// </summary>
	internal static class HelpLinks
	{
		public const string Sudoku001 = "https://gitee.com/SunnieShine/Sudoku/wikis/SUDOKU001?sort_id=3599824";
		public const string Sudoku002 = "https://gitee.com/SunnieShine/Sudoku/wikis/SUDOKU002?sort_id=3599808";
		public const string Sudoku003 = "https://gitee.com/SunnieShine/Sudoku/wikis/SUDOKU003?sort_id=3621783";
		public const string Sudoku004 = "https://gitee.com/SunnieShine/Sudoku/wikis/SUDOKU004?sort_id=3599816";
		public const string Sudoku005 = "https://gitee.com/SunnieShine/Sudoku/wikis/SUDOKU005?sort_id=3599818";
		public const string Sudoku006 = "https://gitee.com/SunnieShine/Sudoku/wikis/SUDOKU006?sort_id=3599826";
		public const string Sudoku007 = "https://gitee.com/SunnieShine/Sudoku/wikis/SUDOKU007?sort_id=3602787";
		public const string Sudoku008 = "https://gitee.com/SunnieShine/Sudoku/wikis/SUDOKU008?sort_id=3607697";
		public const string Sudoku009 = "https://gitee.com/SunnieShine/Sudoku/wikis/SUDOKU009?sort_id=3608009";
		public const string Sudoku010 = "https://gitee.com/SunnieShine/Sudoku/wikis/SUDOKU010?sort_id=3610020";
		public const string Sudoku011 = "https://gitee.com/SunnieShine/Sudoku/wikis/SUDOKU011?sort_id=3610022";
		public const string Sudoku012 = "https://gitee.com/SunnieShine/Sudoku/wikis/SUDOKU012?sort_id=3610347";
		public const string Sudoku013 = "https://gitee.com/SunnieShine/Sudoku/wikis/SUDOKU013?sort_id=3610364";
		public const string Sudoku014 = "https://gitee.com/SunnieShine/Sudoku/wikis/SUDOKU014?sort_id=3614979";
	}

	/// <summary>
	/// Provides the categories.
	/// </summary>
	internal static class Categories
	{
		/// <summary>
		/// Indicates the category is the requirement.
		/// </summary>
		public const string Requirements = nameof(Requirements);

		/// <summary>
		/// Indicates the category is the usage.
		/// </summary>
		public const string Usage = nameof(Usage);

		/// <summary>
		/// Indicates the category is the resource dictionary.
		/// </summary>
		public const string ResourceDictionary = nameof(ResourceDictionary);

		/// <summary>
		/// Indicates the category is the model (data structure implementation).
		/// </summary>
		public const string Model = nameof(Model);
	}

	/// <summary>
	/// Provides the titles.
	/// </summary>
	internal static class Titles
	{
		public const string Sudoku001 = "A property named 'Properties' expected";
		public const string Sudoku002 = "A public property 'Properties' expected";
		public const string Sudoku003 = "The property 'Properties' must be static";
		public const string Sudoku004 = "The property 'Properties' must be read-only";
		public const string Sudoku005 = "The property 'Properties' has a wrong type";
		public const string Sudoku006 = "The property 'Properties' can't be null";
		public const string Sudoku007 = "The property 'Properties' must contain a default value";
		public const string Sudoku008 = "The property 'Properties' must be initialized by a new clause";
		public const string Sudoku009 = "The specified key can't be found in the resource dictionary";
		public const string Sudoku010 = "The specified method can't be found and called";
		public const string Sudoku011 = "The number of arguments dismatched in this dynamically invocation";
		public const string Sudoku012 = "The argument type dismatched in this dynamically invocation";
		public const string Sudoku013 = "The method returns void, but you make it an rvalue expression";
		public const string Sudoku014 = "The member can't be invoked because they are reserved";
	}

	/// <summary>
	/// Provides the messages.
	/// </summary>
	internal static class Messages
	{
		public const string Sudoku001 = "A property named 'Properties' expected.";
		public const string Sudoku002 = "A public property 'Properties' expected.";
		public const string Sudoku003 = "The property 'Properties' must be static.";
		public const string Sudoku004 = "The property 'Properties' must be read-only.";
		public const string Sudoku005 = "The property 'Properties' must be of type 'Sudoku.Solving.Manual.TechiqueProperties'.";
		public const string Sudoku006 = "The property 'Properties' can't be null; try to remove the nullable annotation '?'.";
		public const string Sudoku007 = "The property 'Properties' must contain a default value.";
		public const string Sudoku008 = "The property 'Properties' must be initialized by a new clause; try to initialize it with 'new(...)' or 'new TechniqueProperties(...)' instead.";
		public const string Sudoku009 = "The specified key '{0}' can't be found in the resource dictionary; please check whether your input is valid.";
		public const string Sudoku010 = "The specified method '{0}' can't be found and called; all supported methods are 'Serialize', 'Deserialize' and 'ChangeLanguage'.";
		public const string Sudoku011 = "The method '{0}' expects {1} argument(s), but your input contains {2} argument(s).";
		public const string Sudoku012 = "This argument in the method '{0}' must be of type '{1}', but the actual type is '{2}'.";
		public const string Sudoku013 = "The method '{0}' returns void, but you make it a rvalue expression.";
		public const string Sudoku014 = "The member '{0}' can't be invoked because they are reserved.";
	}
}
