namespace Sudoku.Presentation;

partial struct ValueView
{
	partial struct Enumerator
	{
		/// <summary>
		/// The <see cref="ValueView"/> instance.
		/// </summary>
		private readonly ValueView _instance;

		/// <summary>
		/// The state of the iteration. The value can be both negative and positive.
		/// </summary>
		private int _state;

		/// <summary>
		/// Records the index having been iterated. <b>This field is only used for the case that the current segment is an array.</b>
		/// </summary>
		private int _arrayIteratedIndex;

		/// <summary>
		/// Indicates the array to be iterated. <b>This field is only used for the case that the current segment is an array.</b>
		/// </summary>
		private ViewNode[]? _array;

		/// <summary>
		/// Indicates the target enumerator used for the whole iteration.
		/// </summary>
		private LinkedList<ViewNodeSegment>.Enumerator _nestedEnumerator;

		/// <summary>
		/// Indicates the enumerator of <see cref="List{T}"/>. <b>This field is only used for the case that the current segment is a list.</b>
		/// </summary>
		private List<ViewNode>.Enumerator _listEnumerator;


		/// <inheritdoc cref="IEnumerator.Current"/>
		public ViewNode Current { get; private set; } = null!;

		/// <inheritdoc cref="IEnumerator.MoveNext"/>
		public bool MoveNext()
		{
			ViewNode[]? array;
			List<ViewNode>? list;

#pragma warning disable format
			switch (_state)
			{
				default:
				{
					return false;
				}
				case 0:
				{
					_state = -1;
					_nestedEnumerator = _instance._viewNodeSegements.GetEnumerator();
					_state = -3;

					goto ValueCheckCore;
				}
				case 1:
				{
					_state = -3;

					goto ValueCheckCore;
				}
				case 2:
				{
					_state = -3;
					_arrayIteratedIndex++;

					goto ArrayCheck;
				}
				case 3:
				{
					_state = -4;

					goto ListEnumeratorMoveNext;
				}
				ValueCheckCore:
				{
					while (true)
					{
						if (_nestedEnumerator.MoveNext())
						{
							var a = _nestedEnumerator.Current.ActualValue;
							if (a is ViewNode node)
							{
								Current = node;
								_state = 1;

								return true;
							}

							array = a as ViewNode[];
							if (array is not null)
							{
								break;
							}

							list = a as List<ViewNode>;
							if (list is null)
							{
								continue;
							}

							goto CreateListEnumerator;
						}
						_state = -1;
						_nestedEnumerator = default;
						return false;
					}

					_array = array;
					_arrayIteratedIndex = 0;
					goto ArrayCheck;
				}
				CreateListEnumerator:
				{
					_listEnumerator = list.GetEnumerator();
					_state = -4;

					goto ListEnumeratorMoveNext;
				}
				ListEnumeratorMoveNext:
				{
					if (_listEnumerator.MoveNext())
					{
						Current = _listEnumerator.Current;
						_state = 3;
						return true;
					}

					_state = -3;
					_listEnumerator = default;

					goto ValueCheckCore;
				}
				ArrayCheck:
				{
					if (_arrayIteratedIndex < _array!.Length)
					{
						Current = _array[_arrayIteratedIndex];
						_state = 2;
						return true;
					}
					_array = null;

					goto ValueCheckCore;
				}
			}
#pragma warning restore format
		}

		/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Enumerator GetEnumerator() => this;
	}
}
