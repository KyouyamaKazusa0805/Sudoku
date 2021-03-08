using System.ComponentModel;
using System.Runtime.InteropServices;

namespace System.Text
{
	partial struct ValueStringBuilder
	{
		/// <summary>
		/// <para>
		/// Get a pinnable reference to the builder.
		/// Does not ensure there is a null char after <see cref="Length"/>.
		/// </para>
		/// <para>
		/// This overload is pattern matched in the C# 7.3+ compiler so you can omit
		/// the explicit method call, and write eg <c>fixed (char* c = builder)</c>.
		/// </para>
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public readonly ref readonly char GetPinnableReference() => ref MemoryMarshal.GetReference(_chars);

		/// <summary>
		/// Get a pinnable reference to the builder.
		/// </summary>
		/// <param name="withTerminate">
		/// Ensures that the builder has a null character after <see cref="Length"/>.
		/// </param>
		/// <seealso cref="Length"/>
		public ref readonly char GetPinnableReference(bool withTerminate)
		{
			if (withTerminate)
			{
				EnsureCapacity(Length + 1);
				_chars[Length] = '\0';
			}

			return ref MemoryMarshal.GetReference(_chars);
		}
	}
}
