using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using Sudoku.Globalization;

namespace Sudoku.Windows
{
	/// <summary>
	/// Indicates the resources used later but not in the namespace Sudoku.<see cref="Windows"/>.
	/// </summary>
	/// <seealso cref="Windows"/>
	public static class Resources
	{
		/// <summary>
		/// Indicates the default options.
		/// </summary>
		private static readonly JsonSerializerOptions DefaultOptions = new()
		{
			Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
			WriteIndented = true
		};


		/// <summary>
		/// Indicates the current source.
		/// </summary>
		private static IDictionary<string, string> _dicPointer = null!;


		/// <summary>
		/// Indicates the current country code.
		/// </summary>
		public static CountryCode CountryCode { get; private set; } = CountryCode.EnUs;

		/// <summary>
		/// The language source for the globalization string "<c>en-us</c>".
		/// </summary>
		public static IDictionary<string, string>? LangSourceEnUs { get; internal set; }

		/// <summary>
		/// The language source for the globalization string "<c>zh-cn</c>".
		/// </summary>
		public static IDictionary<string, string>? LangSourceZhCn { get; internal set; }


		/// <summary>
		/// To change the current language with the specified country code.
		/// </summary>
		/// <param name="countryCode">The country code.</param>
		public static void ChangeLanguage(CountryCode countryCode) => GetDictionary(CountryCode = countryCode);

		/// <summary>
		/// Try to serialize the instance whose name is specified as a string.
		/// </summary>
		/// <param name="instanceNameToSerialize">The instance that is to serialize.</param>
		/// <param name="path">The path to serialize.</param>
		/// <returns>The <see cref="bool"/> value indicating whether the operation is successful.</returns>
		public static bool Serialize(string instanceNameToSerialize, string path)
		{
			try
			{
				var propertyInfo = typeof(Resources).GetProperty(instanceNameToSerialize);
				if (propertyInfo is null)
				{
					return false;
				}

				if (propertyInfo.GetValue(null) is not IDictionary<string, string> instance)
				{
					return false;
				}

				string json = JsonSerializer.Serialize(instance, DefaultOptions);
				File.WriteAllText(path, json);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		/// <summary>
		/// Try to deserialize the file specified as a file path, and converts it to the instance.
		/// </summary>
		/// <param name="instanceNameToDeserialize">The instance to covert to.</param>
		/// <param name="path">The file path.</param>
		/// <returns>The <see cref="bool"/> value indicating whether the operaton is successful.</returns>
		public static bool Deserialize(string instanceNameToDeserialize, string path)
		{
			try
			{
				if (!File.Exists(path))
				{
					return false;
				}

				string json = File.ReadAllText(path);
				var instance = JsonSerializer.Deserialize<IDictionary<string, string>>(json, DefaultOptions);
				if (instance is null)
				{
					return false;
				}

				var propertyInfo = typeof(Resources).GetProperty(instanceNameToDeserialize);
				if (propertyInfo is null)
				{
					return false;
				}

				propertyInfo.SetValue(null, instance);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		/// <summary>
		/// Get the value with the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>The value.</returns>
		/// <exception cref="KeyNotFoundException">
		/// Throws when the key can't be found in neither the current language dictionary
		/// nor the default dictionary.
		/// </exception>
		public static string GetValue(string key) =>
			_dicPointer is not null && _dicPointer.TryGetValue(key, out string? result)
			|| LangSourceEnUs is not null && LangSourceEnUs.TryGetValue(key, out result)
			? result
			: throw new KeyNotFoundException();

		/// <summary>
		/// Get the value with the specified key, without any exception throws.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>
		/// The value. If the key can't be found in neither the current language dictionary
		/// nor the default dictionary, the return value will be <see langword="null"/>.
		/// </returns>
		public static string? GetValueWithoutExceptions(string key) =>
			_dicPointer.TryGetValue(key, out string? result)
			|| LangSourceEnUs is not null && LangSourceEnUs.TryGetValue(key, out result)
			? result
			: null;

		/// <summary>
		/// Get the dictionary with the specified globalization string.
		/// </summary>
		/// <param name="countryCode">The country code.</param>
		/// <exception cref="ResourceDictionaryNotFoundException">
		/// Throws when the language resource dictionary doesn't exist (i.e. <see langword="null"/>).
		/// </exception>
		private static void GetDictionary(CountryCode countryCode) =>
			_dicPointer =
				countryCode != CountryCode.Default
				&& typeof(Resources)
				.GetProperty($"LangSource{countryCode}", BindingFlags.Public | BindingFlags.Static)?
				.GetValue(null) is IDictionary<string, string> r
				? r
				: LangSourceEnUs ?? throw new ResourceDictionaryNotFoundException(nameof(LangSourceEnUs));
	}
}
