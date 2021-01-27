using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using Sudoku.DocComments;
using Sudoku.Globalization;

namespace Sudoku.Resources
{
	/// <summary>
	/// Provides a way to get the local text resources, and save the resources to the local path.
	/// </summary>
	public sealed class TextResources : DynamicObject
	{
		/// <summary>
		/// Indicates the default instance.
		/// </summary>
		public static readonly dynamic Current = new TextResources();

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


		/// <inheritdoc cref="DefaultConstructor"/>
		private TextResources()
		{
		}


		/// <summary>
		/// Indicates the current country code.
		/// </summary>
		public CountryCode CountryCode
		{
			get => CurrentCountryCode;

			private set => CurrentCountryCode = value;
		}

		/// <summary>
		/// The language source for the globalization string "<c>en-us</c>".
		/// </summary>
		public IDictionary<string, string>? LangSourceEnUs
		{
			get => CurrentLangSourceEnUs;

			internal set => CurrentLangSourceEnUs = value;
		}

		/// <summary>
		/// The language source for the globalization string "<c>zh-cn</c>".
		/// </summary>
		public IDictionary<string, string>? LangSourceZhCn
		{
			get => CurrentLangSourceZhCn;

			internal set => CurrentLangSourceZhCn = value;
		}

		/// <summary>
		/// Indicates the current country code.
		/// </summary>
		private static CountryCode CurrentCountryCode { get; set; } = CountryCode.EnUs;

		/// <summary>
		/// The language source for the globalization string "<c>en-us</c>".
		/// </summary>
		private static IDictionary<string, string>? CurrentLangSourceEnUs { get; set; }

		/// <summary>
		/// The language source for the globalization string "<c>zh-cn</c>".
		/// </summary>
		private static IDictionary<string, string>? CurrentLangSourceZhCn { get; set; }


		/// <inheritdoc/>
		public override bool TryInvokeMember(InvokeMemberBinder binder, object?[]? args, out object? result)
		{
			switch (binder.Name)
			{
				case nameof(Serialize)
				when args is { Length: 2 } && (args[0], args[1]) is (string instanceName, string path):
				{
					result = null;
					return Serialize(instanceName, path);
				}
				case nameof(Deserialize)
				when args is { Length: 2 } && (args[0], args[1]) is (string instanceName, string path):
				{
					result = null;
					return Deserialize(instanceName, path);
				}
				case nameof(ChangeLanguage) when args is { Length: 1 } && args[0] is CountryCode code:
				{
					result = null;
					ChangeLanguage(code);
					return true;
				}
				default:
				{
					result = null;
					return false;
				}
			}
		}

		/// <inheritdoc/>
		public override bool TryGetMember(GetMemberBinder binder, out object? result)
		{
			switch (binder.Name)
			{
				case nameof(LangSourceEnUs):
				{
					result = CurrentLangSourceEnUs;
					return true;
				}
				case nameof(LangSourceZhCn):
				{
					result = CurrentLangSourceZhCn;
					return true;
				}
				default:
				{
					return (result = TryGetValue(binder.Name)) is not null;
				}
			}
		}

		/// <inheritdoc/>
		public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object? result)
		{
			if (indexes is null || indexes.Length == 0 || indexes[0] is not string index)
			{
				result = null;
				return false;
			}

			return (result = TryGetValue(index)) is not null;
		}


		/// <summary>
		/// Try to deserialize the file specified as a file path, and converts it to the instance.
		/// </summary>
		/// <param name="instanceNameToDeserialize">The instance to covert to.</param>
		/// <param name="path">The file path.</param>
		/// <returns>The <see cref="bool"/> value indicating whether the operaton is successful.</returns>
		internal static bool Deserialize(string instanceNameToDeserialize, string path)
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

				var propertyInfo = typeof(TextResources).GetProperty(
					$"Current{instanceNameToDeserialize}",
					BindingFlags.Static | BindingFlags.NonPublic
				);
				if (propertyInfo is null)
				{
					return false;
				}

				propertyInfo.SetValue(null, instance);
				return true;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// Try to serialize the instance whose name is specified as a string.
		/// </summary>
		/// <param name="instanceNameToSerialize">The instance that is to serialize.</param>
		/// <param name="path">The path to serialize.</param>
		/// <returns>The <see cref="bool"/> value indicating whether the operation is successful.</returns>
		private static bool Serialize(string instanceNameToSerialize, string path)
		{
			try
			{
				var propertyInfo = typeof(TextResources).GetProperty($"Current{instanceNameToSerialize}");
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
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// To change the current language with the specified country code.
		/// </summary>
		/// <param name="countryCode">The country code.</param>
		private static void ChangeLanguage(CountryCode countryCode)
		{
			CurrentCountryCode = countryCode;

			var defaultDictionary = CurrentLangSourceEnUs ?? throw new SudokuHandlingException<string>(errorCode: 403, nameof(LangSourceEnUs));
			if (countryCode == CountryCode.Default)
			{
				goto DefaultAssignment;
			}

			if
			(
				typeof(TextResources).GetProperty(
					$"CurrentLangSource{countryCode.ToString()}",
					BindingFlags.NonPublic | BindingFlags.Static
				) is not { } propInfo
			)
			{
				goto DefaultAssignment;
			}

			if (propInfo.GetValue(null) is not IDictionary<string, string> r)
			{
				goto DefaultAssignment;
			}

			_dicPointer = r;
			return;

		DefaultAssignment:
			_dicPointer = defaultDictionary;
		}

		/// <summary>
		/// Get the value with the specified key, without any exception throws.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>
		/// The value. If the key can't be found in neither the current language dictionary
		/// nor the default dictionary, the return value will be <see langword="null"/>.
		/// </returns>
		private static string? TryGetValue(string key) =>
			_dicPointer.TryGetValue(key, out string? result)
			? result
			: CurrentLangSourceEnUs?.TryGetValue(key, out result) ?? false ? result : null;
	}
}
