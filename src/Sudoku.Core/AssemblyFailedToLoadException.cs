using System;
using System.Runtime.Serialization;

namespace Sudoku
{
	/// <summary>
	/// Indicates an exception that throws when the module initializer failed to fetch the resource data,
	/// which causes the assembly can't be loaded successfully.
	/// </summary>
	[Serializable]
	public sealed class AssemblyFailedToLoadException : Exception
	{
		/// <summary>
		/// Initializes a <see cref="AssemblyFailedToLoadException"/> instance with the specified assembly name
		/// and the file path.
		/// </summary>
		/// <param name="assemblyName">The assembly name.</param>
		/// <param name="filePath">The file path.</param>
		public AssemblyFailedToLoadException(string assemblyName, string filePath)
		{
			AssemblyName = assemblyName;
			FilePath = filePath;
			Data.Add(nameof(AssemblyName), assemblyName);
			Data.Add(nameof(filePath), filePath);
		}

		/// <inheritdoc/>
		private AssemblyFailedToLoadException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}


		/// <summary>
		/// Indicates the assembly name.
		/// </summary>
		public string? AssemblyName { get; }

		/// <summary>
		/// Indicates the file path.
		/// </summary>
		public string? FilePath { get; }

		/// <inheritdoc/>
		public override string Message =>
			$"The assembly {AssemblyName} can't be loaded. " +
			$"The large possibility of the problem raised is that the required files don't exist. " +
			$"Please check the existence of the resource dictionary file (path {FilePath}).";

		/// <inheritdoc/>
		public override string HelpLink =>
			"https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3707152&doc_id=633030";


		/// <inheritdoc/>
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue(nameof(AssemblyName), AssemblyName, typeof(string));
			info.AddValue(nameof(FilePath), FilePath, typeof(string));

			base.GetObjectData(info, context);
		}
	}
}
