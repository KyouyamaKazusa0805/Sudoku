using System;
using System.Runtime.Serialization;

namespace Sudoku
{
	/// <summary>
	/// Indicates the exception that throws when the resource can't be found.
	/// </summary>
	[Serializable]
	public sealed class ResourceCannotBeFoundException : Exception
	{
		/// <summary>
		/// Initializes a <see cref="ResourceCannotBeFoundException"/> instance with the specified resource.
		/// </summary>
		/// <param name="resource">The resource.</param>
		public ResourceCannotBeFoundException(string resource)
		{
			Resource = resource;
			Data.Add(nameof(Resource), resource);
		}

		/// <inheritdoc/>
		private ResourceCannotBeFoundException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}


		/// <summary>
		/// Indicates the resource file.
		/// </summary>
		public string? Resource { get; }

		/// <inheritdoc/>
		public override string Message =>
			$"The specified resource dictionary name can't be found: {Resource}.";

		/// <inheritdoc/>
		public override string HelpLink =>
			"https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3707103&doc_id=633030";


		/// <inheritdoc/>
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue(nameof(Resource), Resource, typeof(string));

			base.GetObjectData(info, context);
		}
	}
}
