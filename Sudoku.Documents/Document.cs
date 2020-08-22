using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Sudoku.Documents
{
	/// <summary>
	/// Indicates an XML document (doc comment file).
	/// </summary>
	public sealed class Document : IEnumerable<XmlNode>
	{
		/// <summary>
		/// The document instance.
		/// </summary>
		private readonly XmlDocument _document = new();


		/// <summary>
		/// Initializes an instance with the specified path.
		/// </summary>
		/// <param name="path">The path of the document.</param>
		/// <exception cref="FileLoadException">Throws when the path isn't end with <c>.xml</c>.</exception>
		/// <exception cref="FileNotFoundException">Throws when the file cannot be found.</exception>
		public Document(string path)
		{
			if (!path.EndsWith(".xml"))
			{
				throw new FileLoadException();
			}

			if (!File.Exists(path))
			{
				throw new FileNotFoundException("The specified file cannot be found.", path);
			}

			_document.LoadXml(File.ReadAllText(path));
			Root = _document.SelectSingleNode("doc");
		}


		/// <summary>
		/// Indicates the root node.
		/// </summary>
		public XmlNode Root { get; }


		/// <summary>
		/// Gets the string value of the specified node.
		/// </summary>
		/// <param name="attribute">The node name.</param>
		/// <returns>The string value. Returns <see langword="null"/> if not found.</returns>
		public XmlNode? this[string attribute] => Root[attribute];


		/// <inheritdoc/>
		public IEnumerator<XmlNode> GetEnumerator()
		{
			return r(Root).GetEnumerator();

			static IEnumerable<XmlNode> r(XmlNode node)
			{
				if (node.HasChildNodes)
				{
					foreach (XmlNode? subNode in node.ChildNodes)
					{
						if (subNode is not null)
						{
							yield return subNode;

							foreach (var n in r(subNode))
							{
								yield return n;
							}
						}
					}
				}
			}
		}

		/// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
