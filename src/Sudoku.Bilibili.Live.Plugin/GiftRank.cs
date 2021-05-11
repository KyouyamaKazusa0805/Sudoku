using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Sudoku.Bilibili.Live.Plugin
{
	/// <summary>
	/// Indicates a gift ranking result.
	/// </summary>
	public sealed class GiftRank : INotifyPropertyChanged
	{
		/// <summary>
		/// Indicates the user name.
		/// </summary>
		private string? _userName;

		/// <summary>
		/// Indicates the cost.
		/// </summary>
		private decimal _cost;

		/// <summary>
		/// Indicates the UID of that user.
		/// </summary>
		private int _uid;


		/// <summary>
		/// Indicates the user name.
		/// </summary>
		/// <value>The user name.</value>
		/// <returns>The user name.</returns>
		[DisallowNull]
		public string? UserName
		{
			get => _userName;

			set
			{
				if (value != _userName)
				{
					_userName = value;
					OnPropertyChanged(nameof(UserName));
				}
			}
		}

		/// <summary>
		/// Gets or sets the cost.
		/// </summary>
		/// <value>The cost.</value>
		/// <returns>The cost.</returns>
		public decimal Cost
		{
			get => _cost;

			set
			{
				if (value != _cost)
				{
					_cost = value;
					OnPropertyChanged(nameof(Cost));
				}
			}
		}

		/// <summary>
		/// Gets or sets the UID of that user.
		/// </summary>
		/// <value>The UID value.</value>
		/// <returns>The UID value.</returns>
		public int Uid
		{
			get => _uid;

			set
			{
				if (value != _uid)
				{
					_uid = value;
					OnPropertyChanged(nameof(Uid));
				}
			}
		}


		/// <summary>
		/// Indicates the event to be triggered when a property has changed its status.
		/// </summary>
		public event PropertyChangedEventHandler? PropertyChanged;


		/// <summary>
		/// To trigger the event <see cref="PropertyChanged"/>.
		/// </summary>
		/// <param name="propertyName">The property name changed its status.</param>
		/// <seealso cref="PropertyChanged"/>
		private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
