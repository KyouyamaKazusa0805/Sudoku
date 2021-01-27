using System.Collections.Generic;
using System.Dynamic;
using Sudoku.Globalization;

namespace Sudoku.Resources
{
	/// <summary>
	/// Provides a way to get the local text resources, and save the resources to the local path.
	/// </summary>
	public sealed partial class TextResources : DynamicObject
	{
		/// <summary>
		/// Indicates the singleton to get items and method invokes.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This is a <see langword="dynamic"/> instance, which means you can get anything you want
		/// using the following code style to get the items in this class:
		/// <list type="bullet">
		/// <item><c>Current.PropertyName</c> (Property invokes)</item>
		/// <item><c>Current[PropertIndex]</c> (Indexer invokes)</item>
		/// <item><c>Current.MethodName(Parameters)</c> (Method invokes)</item>
		/// </list>
		/// </para>
		/// <para>
		/// For example,
		/// if you want to get the <see cref="string"/> value from the key <c>"Bug"</c>, now you may
		/// write <c>Current.Bug</c> or <c>Current["Bug"]</c> to get that value.
		/// Before the version 0.3, you must write the code <c>TextResources.GetValue("Bug")</c>.
		/// </para>
		/// <para>
		/// All supported methods are:
		/// <list type="bullet">
		/// <item><c>Current.Serialize(string instanceNameToSerialize, string toPath)</c></item>
		/// <item><c>Current.Deserialize(string instanceNameToDeserialize, string fromPath)</c></item>
		/// <item><c>Current.ChangeLanguage(CountryCode countryCode)</c></item>
		/// </list>
		/// </para>
		/// </remarks>
		public static readonly dynamic Current = new TextResources();


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
	}
}
