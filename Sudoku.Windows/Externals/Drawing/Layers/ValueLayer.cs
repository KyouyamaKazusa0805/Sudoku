using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Extensions;

namespace Sudoku.Drawing.Layers
{
	/// <summary>
	/// Provides a value layer.
	/// </summary>
	[DebuggerStepThrough]
	public sealed class ValueLayer : Layer
	{
		/// <summary>
		/// Indicates the value scale.
		/// </summary>
		private readonly decimal _valueScale;

		/// <summary>
		/// Indicates the candidate scale.
		/// </summary>
		private readonly decimal _candidateScale;

		/// <summary>
		/// Indicates whether the layer will display candidates.
		/// </summary>
		private readonly bool _showCandidates;

		/// <summary>
		/// Indicates the color of the status of each digit.
		/// </summary>
		private readonly Color _givenColor, _modifiableColor, _candidateColor;

		/// <summary>
		/// Indicates the grid.
		/// </summary>
		private readonly Grid _grid;

		/// <summary>
		/// Indicates the font name of the status of each digit.
		/// </summary>
		private readonly string _givenFont, _modifiableFont, _candidateFont;


		/// <summary>
		/// Initializes an instance with the value and candidate scales,
		/// the given, modifiable and candidate colors and a grid.
		/// </summary>
		/// <param name="pointConverter">The point converter.</param>
		/// <param name="valueScale">The scale of values.</param>
		/// <param name="candidateScale">The scale of candidates.</param>
		/// <param name="givenColor">The given color.</param>
		/// <param name="modifiableColor">The modifiable color.</param>
		/// <param name="candidateColor">The candidate color.</param>
		/// <param name="givenFont">The given font.</param>
		/// <param name="modifiableFont">The modifiable font.</param>
		/// <param name="candidateFont">The candidate font.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="showCandidates">Indicates whether the layer will show the candidates.</param>
		public ValueLayer(
			PointConverter pointConverter, decimal valueScale, decimal candidateScale,
			Color givenColor, Color modifiableColor, Color candidateColor,
			string givenFont, string modifiableFont, string candidateFont, Grid grid,
			bool showCandidates) : base(pointConverter)
		{
			_valueScale = valueScale;
			_candidateScale = candidateScale;
			_givenColor = givenColor;
			_modifiableColor = modifiableColor;
			_candidateColor = candidateColor;
			_givenFont = givenFont;
			_modifiableFont = modifiableFont;
			_candidateFont = candidateFont;
			_grid = grid;
			_showCandidates = showCandidates;
		}


		/// <inheritdoc/>
		public override int Priority => 100;


		/// <inheritdoc/>
		protected override void Draw()
		{
			float cellWidth = _pointConverter.CellSize.Width;
			float candidateWidth = _pointConverter.CandidateSize.Width;
			float vOffsetValue = cellWidth / 9; // The vertical offset of rendering each value.
			float vOffsetCandidate = candidateWidth / 9; // The vertical offset of rendering each candidate.

			var result = new Bitmap((int)Width, (int)Height);
			using var g = Graphics.FromImage(result);
			using var bGiven = new SolidBrush(_givenColor);
			using var bModifiable = new SolidBrush(_modifiableColor);
			using var bCandidate = new SolidBrush(_candidateColor);
			using var fGiven = GetFontByScale(_givenFont, cellWidth / 2F, _valueScale);
			using var fModifiable = GetFontByScale(_modifiableFont, cellWidth / 2F, _valueScale);
			using var fCandidate = GetFontByScale(_candidateFont, cellWidth / 2F, _candidateScale);
			g.CompositingQuality = CompositingQuality.HighQuality;
			g.SmoothingMode = SmoothingMode.HighQuality;
			g.TextRenderingHint = TextRenderingHint.AntiAlias;

			for (int cell = 0; cell < 81; cell++)
			{
				short mask = _grid.GetMask(cell);

				// Firstly, draw values.
				var status = (CellStatus)(mask >> 9 & (int)CellStatus.All);
				switch (status)
				{
					case CellStatus.Empty when _showCandidates:
					{
						// Draw candidates.
						short candidateMask = (short)(~mask & Grid.MaxCandidatesMask);
						foreach (int digit in candidateMask.GetAllSets())
						{
							var point = _pointConverter.GetMousePointInCenter(cell, digit);
							point.Y += vOffsetCandidate;
							g.DrawString(
								s: (digit + 1).ToString(),
								font: fCandidate,
								brush: bCandidate,
								point,
								format: DefaultStringFormat);
						}

						break;
					}
					case CellStatus.Modifiable:
					case CellStatus.Given:
					{
						// Draw values.
						var point = _pointConverter.GetMousePointInCenter(cell);
						point.Y += vOffsetValue;
						g.DrawString(
							s: (_grid[cell] + 1).ToString(),
							font: status == CellStatus.Given ? fGiven : fModifiable,
							brush: status == CellStatus.Given ? bGiven : bModifiable,
							point,
							format: DefaultStringFormat);

						break;
					}
				}
			}

			Target = result;
		}

		/// <summary>
		/// Get the font via name, size and the scale.
		/// </summary>
		/// <param name="fontName">The font name.</param>
		/// <param name="size">The size.</param>
		/// <param name="scale">The scale.</param>
		/// <returns>The font.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Font GetFontByScale(string fontName, float size, decimal scale) =>
			new Font(fontName, size * (float)scale, FontStyle.Regular);
	}
}
