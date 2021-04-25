using System;
using System.Text;
using Sudoku.CodeGen.Equality.Annotations;
using Sudoku.CodeGen.HashCode.Annotations;
using Sudoku.DocComments;
using Sudoku.UI.ResourceDictionaries;

namespace Sudoku.UI.Extensions
{
	/// <summary>
	/// Encapsulates a filter that stores a several of file formats.
	/// </summary>
	[AutoEquality(nameof(ResultStr))]
	[AutoHashCode(nameof(ResultStr))]
	public sealed partial class Filter : IEquatable<Filter?>
	{
		/// <summary>
		/// Indicates the inner string builder.
		/// </summary>
		private readonly StringBuilder _sb;


		/// <inheritdoc cref="DefaultConstructor"/>
		public Filter() => _sb = new();


		/// <summary>
		/// Indicates the number of file formats the instance recorded.
		/// </summary>
		public int Count { get; private set; }

		/// <summary>
		/// Indicates the result string value.
		/// </summary>
		private string ResultStr => _sb.ToString();


		/// <summary>
		/// Append the JPG format filter into the list.
		/// </summary>
		/// <returns>The reference of <see langword="this"/>.</returns>
		public Filter WithJpg()
		{
			AddSeparatorIfNeed();
			AddFormat(TextResources.Current.FilterNameJpg, "*.jpg");
			return this;
		}

		/// <summary>
		/// Append the PNG format filter into the list.
		/// </summary>
		/// <returns>The reference of <see langword="this"/>.</returns>
		public Filter WithPng()
		{
			AddSeparatorIfNeed();
			AddFormat(TextResources.Current.FilterNamePng, "*.png");
			return this;
		}

		/// <summary>
		/// Append the GIF format filter into the list.
		/// </summary>
		/// <returns>The reference of <see langword="this"/>.</returns>
		public Filter WithGif()
		{
			AddSeparatorIfNeed();
			AddFormat(TextResources.Current.FilterNameGif, "*.gif");
			return this;
		}

		/// <summary>
		/// Append the BMP format filter into the list.
		/// </summary>
		/// <returns>The reference of <see langword="this"/>.</returns>
		public Filter WithBmp()
		{
			AddSeparatorIfNeed();
			AddFormat(TextResources.Current.FilterNameBmp, "*.bmp");
			return this;
		}

		/// <summary>
		/// Append the WMF format filter into the list.
		/// </summary>
		/// <returns>The reference of <see langword="this"/>.</returns>
		public Filter WithWmf()
		{
			AddSeparatorIfNeed();
			AddFormat(TextResources.Current.FilterNameWmf, "*.wmf");
			return this;
		}

		/// <summary>
		/// Append the JSON format filter into the list.
		/// </summary>
		/// <returns>The reference of <see langword="this"/>.</returns>
		public Filter WithJson()
		{
			AddSeparatorIfNeed();
			AddFormat(TextResources.Current.FilterNameJson, "*.json");
			return this;
		}

		/// <summary>
		/// Append the text format filter into the list.
		/// </summary>
		/// <returns>The reference of <see langword="this"/>.</returns>
		public Filter WithText()
		{
			AddSeparatorIfNeed();
			AddFormat(TextResources.Current.FilterNameTxt, "*.txt");
			return this;
		}

		/// <summary>
		/// Append the sudoku format filter into the list.
		/// </summary>
		/// <returns>The reference of <see langword="this"/>.</returns>
		public Filter WithSudoku()
		{
			AddSeparatorIfNeed();
			AddFormat(TextResources.Current.FilterNameSudoku, "*.sudoku");
			return this;
		}

		/// <summary>
		/// Append the sudoku database format filter into the list.
		/// </summary>
		/// <returns>The reference of <see langword="this"/>.</returns>
		public Filter WithSudokuDatabase()
		{
			AddSeparatorIfNeed();
			AddFormat(TextResources.Current.FilterNameSudokus, "*.sudokus");
			return this;
		}

		/// <summary>
		/// Append the sudoku drawing contents format filter into the list.
		/// </summary>
		/// <returns>The reference of <see langword="this"/>.</returns>
		public Filter WithDrawings()
		{
			AddSeparatorIfNeed();
			AddFormat(TextResources.Current.FilterNameDrawings, "*.drawings");
			return this;
		}

		/// <summary>
		/// Append the sudoku configuration format filter into the list.
		/// </summary>
		/// <returns>The reference of <see langword="this"/>.</returns>
		public Filter WithSudokuConfig()
		{
			AddSeparatorIfNeed();
			AddFormat(TextResources.Current.FilterNameScfg, "*.scfg");
			return this;
		}

		/// <summary>
		/// Append the all-file format filter into the list.
		/// </summary>
		/// <returns>The reference of <see langword="this"/>.</returns>
		public Filter WithAll()
		{
			AddSeparatorIfNeed();
			AddFormat(TextResources.Current.FilterNameAll, "*.*");
			return this;
		}

		/// <inheritdoc/>
		public override string ToString() => ResultStr;

		/// <summary>
		/// Add the format.
		/// </summary>
		/// <param name="filterName">The filter name.</param>
		/// <param name="filter">The filter.</param>
		private void AddFormat(string filterName, string filter) =>
			_sb.Append(filterName).Append('|').Append(filter);

		/// <summary>
		/// Add a separator if need.
		/// </summary>
		private void AddSeparatorIfNeed()
		{
			if (++Count != 1)
			{
				_sb.Append('|');
			}
		}


		/// <inheritdoc cref="Operators.operator =="/>
		public static bool operator ==(Filter left, Filter right) => left.Equals(right);

		/// <inheritdoc cref="Operators.operator !="/>
		public static bool operator !=(Filter left, Filter right) => !(left == right);


		/// <summary>
		/// Implicit cast from <see cref="Filter"/> to <see cref="string"/>.
		/// </summary>
		/// <param name="filter">The filter.</param>
		public static implicit operator string(Filter filter) => filter.ToString();
	}
}
