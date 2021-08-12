using System.Text;
using System.Xml;

namespace Sudoku.Diagnostics
{
	/// <summary>
	/// Defines a C# project file <c>*.csproj</c> information updater.
	/// </summary>
	public sealed class CSharpProjectUpdater
	{
		/// <summary>
		/// Initializes a <see cref="CSharpProjectUpdater"/> instance with the specified solution path.
		/// </summary>
		/// <param name="solutionPath">The solution path.</param>
		public CSharpProjectUpdater(string solutionPath) => SolutionPath = solutionPath;


		/// <summary>
		/// Indicates the solution path.
		/// </summary>
		public string SolutionPath { get; }


		/// <summary>
		/// To increase <c><![CDATA[<Version>]]></c> block by 0.1.
		/// </summary>
		/// <param name="leadingString">The leading string.</param>
		/// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
		/// <returns>The task.</returns>
		public async Task IncreaseVersionAsync(
			string? leadingString = null, CancellationToken cancellationToken = default)
		{
			var fileCounter = new FileCounter(SolutionPath, "csproj", false);
			await fileCounter.CountUpAsync(cancellationToken);

			foreach (string file in fileCounter.FileList)
			{
				var xmlDocument = new XmlDocument();
				xmlDocument.Load(file);

				var root = xmlDocument.DocumentElement;
				if (root is null)
				{
					continue;
				}

				if (root.SelectNodes("descendant::PropertyGroup") is not { } propertyGroupList)
				{
					continue;
				}

				if (propertyGroupList.Cast<XmlNode>().FirstOrDefault() is not { } current)
				{
					continue;
				}

				var element = current;
				while ((element = element!.NextSibling) is not null)
				{
					if (element.Name == "Version" && decimal.TryParse(element.Value, out decimal result))
					{
						element.Value = (result + .1M).ToString();
						goto NextLoop;
					}
				}

				var elem = xmlDocument.CreateElement("Version");
				elem.InnerText = "0.1";

				current.AppendChild(elem);

			NextLoop:
				xmlDocument.Save(file);

				if (leadingString is not null)
				{
					string text = File.ReadAllText(file).Replace("  ", leadingString);
					File.WriteAllText(file, text, Encoding.UTF8);
				}
			}
		}

		/// <summary>
		/// To update <c><![CDATA[<Nullable>]]></c> block.
		/// </summary>
		/// <param name="nullableValue">The nullable value to update.</param>
		/// <param name="leadingString">The leading string.</param>
		/// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
		/// <returns>The task.</returns>
		public async Task UpdateNullableAsync(
			string nullableValue, string? leadingString = null,
			CancellationToken cancellationToken = default)
		{
			var fileCounter = new FileCounter(SolutionPath, "csproj", false);
			await fileCounter.CountUpAsync(cancellationToken);

			foreach (string file in fileCounter.FileList)
			{
				var xmlDocument = new XmlDocument();
				xmlDocument.Load(file);

				var root = xmlDocument.DocumentElement;
				if (root is null)
				{
					continue;
				}

				if (root.SelectNodes("descendant::PropertyGroup") is not { } propertyGroupList)
				{
					continue;
				}

				if (propertyGroupList.Cast<XmlNode>().FirstOrDefault() is not { } current)
				{
					continue;
				}

				var element = current;
				while ((element = element!.NextSibling) is not null)
				{
					if (element.Name == "Nullable")
					{
						element.Value = nullableValue;
						goto NextLoop;
					}
				}

				var elem = xmlDocument.CreateElement("Nullable");
				elem.InnerText = nullableValue;

				current.AppendChild(elem);

			NextLoop:
				xmlDocument.Save(file);

				if (leadingString is not null)
				{
					string text = File.ReadAllText(file).Replace("  ", leadingString);
					File.WriteAllText(file, text, Encoding.UTF8);
				}
			}
		}
	}
}
