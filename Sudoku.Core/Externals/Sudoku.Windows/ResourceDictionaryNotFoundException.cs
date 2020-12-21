using System;

namespace Sudoku.Windows
{
	/// <summary>
	/// Indicates an exception that throws when the resource dictionary doesn't exist.
	/// </summary>
	[Serializable]
	public sealed class ResourceDictionaryNotFoundException : Exception
	{
		/// <summary>
		/// Initializes an instance with the default behavior.
		/// </summary>
		public ResourceDictionaryNotFoundException()
		{
		}

		/// <summary>
		/// Initializes an instance with the name of the resource dictionary.
		/// </summary>
		/// <param name="resourceDictionaryName">The name of the resource dictionary.</param>
		public ResourceDictionaryNotFoundException(string? resourceDictionaryName) =>
			ResourceDictionaryName = resourceDictionaryName;


		/// <summary>
		/// Initializes an instance with the specified message and the name of the resource dictionary.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="resourceDictionaryName">The name of the resource dictionary.</param>
		public ResourceDictionaryNotFoundException(string message, string? resourceDictionaryName) : base(message) =>
			ResourceDictionaryName = resourceDictionaryName;

		/// <summary>
		/// Initializes an instance with the specified message, the name of the resource dictionary
		/// and the inner exception.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="resourceDictionaryName">The name of the resource dictionary.</param>
		/// <param name="inner">The inner exception.</param>
		public ResourceDictionaryNotFoundException(
			string message, string? resourceDictionaryName, Exception inner) : base(message, inner) =>
			ResourceDictionaryName = resourceDictionaryName;


		/// <summary>
		/// Indicates the resource dictionary name.
		/// </summary>
		public string? ResourceDictionaryName { get; }

		/// <inheritdoc/>
		public override string Message =>
			$"The dictionary doesn't found: {ResourceDictionaryName ?? string.Empty}";
	}
}
