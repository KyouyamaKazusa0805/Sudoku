using System.Text.Json;
using Sudoku.DocComments;
using Sudoku.Drawing.Converters;

namespace Sudoku.Bot.Serialization
{
	/// <summary>
	/// Provides the instances that is <see cref="JsonSerializerOptions"/>.
	/// </summary>
	public static class SerializationOptions
	{
		/// <summary>
		/// The options that is used for the serialization and deserialization.
		/// </summary>
		public static readonly JsonSerializerOptions Default;


		/// <inheritdoc cref="StaticConstructor"/>
		static SerializationOptions()
		{
			var jsonOptions = new JsonSerializerOptions() { WriteIndented = true };
			jsonOptions.Converters.Add(new ColorJsonConverter());

			Default = jsonOptions;
		}
	}
}
