using System;
using Sudoku.Bilibili.Live.Danmaku.EndianBitConverters;

namespace Sudoku.Bilibili.Live.Danmaku
{
	/// <summary>
	/// Encapsulates a danmaku protocol.
	/// </summary>
	public readonly struct DanmakuProtocol
	{
		/// <summary>
		/// Initializes an instance with the packet length, header length, version, action and the parameter.
		/// </summary>
		/// <param name="packetLength">The packet length.</param>
		/// <param name="headerLength">The header length.</param>
		/// <param name="version">The version.</param>
		/// <param name="action">The action.</param>
		/// <param name="parameter">The parameter.</param>
		public DanmakuProtocol(int packetLength, short headerLength, short version, int action, int parameter)
		{
			PacketLength = packetLength;
			HeaderLength = headerLength;
			Version = version;
			Action = action;
			Parameter = parameter;
		}


		/// <summary>
		/// Indicates the total length of the message.
		/// The value is always the sum of the header length, and the data length.
		/// </summary>
		public int PacketLength { get; init; }

		/// <summary>
		/// Indicates the total length of the header.
		/// The value is always <c><see langword="sizeof"/>(<see cref="DanmakuProtocol"/>)</c>.
		/// </summary>
		public short HeaderLength { get; init; }

		/// <summary>
		/// Indicates the version of the message.
		/// </summary>
		public short Version { get; init; }

		/// <summary>
		/// Indicates the message type.
		/// </summary>
		public int Action { get; init; }

		/// <summary>
		/// Indicates the parameter value. The value always keeps the value <c>1</c>.
		/// </summary>
		public int Parameter { get; init; }


		/// <summary>
		/// Creates a <see cref="DanmakuProtocol"/> with the specified buffer.
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		/// <returns>The instance.</returns>
		/// <exception cref="ArgumentException">Throws when the buffer length is lower than 16.</exception>
		public static DanmakuProtocol FromBuffer(byte[] buffer) => buffer.Length < 16
			? throw new ArgumentException("The specified buffer can't lower than 16 of length.", nameof(buffer))
			: new(
				EndianBitConverter.BigEndian.ToInt32(buffer, 0),
				EndianBitConverter.BigEndian.ToInt16(buffer, 4),
				EndianBitConverter.BigEndian.ToInt16(buffer, 6),
				EndianBitConverter.BigEndian.ToInt32(buffer, 8),
				EndianBitConverter.BigEndian.ToInt32(buffer, 12)
			);
	}
}
