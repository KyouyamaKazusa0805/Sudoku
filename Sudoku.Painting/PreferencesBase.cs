using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Sudoku.DocComments;
using Sudoku.JsonConverters;
using Sudoku.Painting.JsonConverters;

namespace Sudoku.Painting
{
	/// <summary>
	/// Provides the basic settings used for painting controls.
	/// </summary>
	public abstract partial class PreferencesBase
	{
		/// <summary>
		/// Indicates the serialization and deserialization option;
		/// </summary>
		private static readonly JsonSerializerOptions Options;


		/// <inheritdoc cref="DefaultConstructor"/>
		public PreferencesBase()
		{
		}

		/// <summary>
		/// Initializes an instance with the specified path to deserialize the file.
		/// </summary>
		/// <param name="path">The path.</param>
		public PreferencesBase(string path)
		{
			if (Deserialize(path, out var instance))
			{
				instance.CopyTo(this);
			}
		}


		/// <inheritdoc cref="StaticConstructor"/>
		static PreferencesBase()
		{
			Options = new() { WriteIndented = true };
			Options.Converters.Add(new ColorJsonConverter());
			Options.Converters.Add(new SudokuGridJsonConverter());
			Options.Converters.Add(new PresentationDataJsonConverter());
			Options.Converters.Add(new DrawingInfoJsonConverter());
			Options.Converters.Add(new CellsJsonConverter());
			Options.Converters.Add(new LinkJsonConverter());
			Options.Converters.Add(new DirectLineJsonConverter());
		}


		/// <summary>
		/// Copy all values in this current instance to the specified instance.
		/// </summary>
		/// <param name="instance">The instance to receive all values.</param>
		public void CopyTo(PreferencesBase instance)
		{
			foreach (var (prop, value) in GetPropertyInfos())
			{
				prop.SetValue(instance, value);
			}
		}

		/// <summary>
		/// To get all possible property information instances.
		/// </summary>
		/// <returns>All preference items.</returns>
		internal IEnumerable<(PropertyInfo Info, object Value)> GetPropertyInfos() =>
			from prop in typeof(PreferencesBase).GetProperties()
			where prop.CanWrite
			let value = typeof(PreferencesBase).GetProperty(prop.Name)!.GetValue(this)!
			select (prop, value);


		/// <summary>
		/// Try to serialize the current instance to a local path.
		/// </summary>
		/// <param name="instance">The instance to serialize.</param>
		/// <param name="path">The path to store the serialization result.</param>
		/// <returns>Returns whether the operation is successful.</returns>
		public static bool Serialize(PreferencesBase instance, string path)
		{
			try
			{
				string? dir = Path.GetDirectoryName(path);
				if (dir is null)
				{
					return false;
				}

				string json = JsonSerializer.Serialize(instance, Options);
				if (!Directory.Exists(dir))
				{
					Directory.CreateDirectory(dir);
				}

				File.WriteAllText(path, json);
				return true;
			}
			catch (JsonException)
			{
				return false;
			}
		}

		/// <summary>
		/// Try to deserialize the file from a local path, and converts to the current type instance.
		/// </summary>
		/// <param name="path">The path that stores the serialization result.</param>
		/// <param name="result">
		/// The instance that received and converted. If failed to convert,
		/// the value will be <see langword="null"/>.
		/// </param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		public static bool Deserialize(string path, [NotNullWhen(true)] out PreferencesBase? result)
		{
			if (!File.Exists(path))
			{
				result = null;
				return false;
			}

			try
			{
				string json = File.ReadAllText(path);
				return (result = JsonSerializer.Deserialize<PreferencesBase>(json, Options)) is not null;
			}
			catch (JsonException)
			{
				result = null;
				return false;
			}
		}
	}
}
