using System.Runtime.Serialization;

namespace System.Text.RegularExpressions
{
	/// <summary>
	/// Indicates an error for reporting a string is an invalid regular expression.
	/// </summary>
	public class InvalidRegexStringException : ArgumentException
	{
		/// <inheritdoc/>
		public InvalidRegexStringException() : base() { }

		/// <inheritdoc/>
		public InvalidRegexStringException(string message) : base(message) { }

		/// <inheritdoc/>
		public InvalidRegexStringException(string message, string paramName) : base(message, paramName) { }

		/// <inheritdoc/>
		public InvalidRegexStringException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <inheritdoc/>
		public InvalidRegexStringException(string message, string paramName, Exception innerException)
			: base(message, paramName, innerException)
		{
		}

		/// <inheritdoc/>
		protected InvalidRegexStringException(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
		}


		/// <inheritdoc/>
		public override string Message => "The specified argument is invalid regular expression.";
	}
}
