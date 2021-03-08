namespace System.Text
{
	partial struct ValueStringBuilder
	{
		/// <summary>
		/// Try to copy the current instance to the specified builder.
		/// </summary>
		/// <param name="builder">(<see langword="ref"/> parameter) The builder.</param>
		/// <exception cref="ArgumentException">
		/// Throws when the target argument doesn't contain the enough space.
		/// </exception>
		public readonly unsafe void CopyTo(ref ValueStringBuilder builder)
		{
			if (builder.Capacity < Length)
			{
				throw new ArgumentException(
					"The argument can't receive the value because the instance doesn't contain the enough space.",
					nameof(builder)
				);
			}

			fixed (char* pThis = _chars, pBuilder = builder._chars)
			{
				int i = 0;
				for (char* p = pThis, q = pBuilder; i < Length; i++)
				{
					*p++ = *q++;
				}
			}

			builder.Length = Length;
		}

		/// <summary>
		/// Try to copy the current instance to the specified builder.
		/// </summary>
		/// <param name="builder">The builder.</param>
		public readonly void CopyTo(StringBuilder builder)
		{
			builder.Length = Length;

			for (int i = 0; i < Length; i++)
			{
				builder[i] = _chars[i];
			}
		}
	}
}
