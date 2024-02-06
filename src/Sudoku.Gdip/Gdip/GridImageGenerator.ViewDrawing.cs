namespace Sudoku.Gdip;

public partial class GridImageGenerator
{
	/// <summary>
	/// Draw custom view if <see cref="View"/> is not <see langword="null"/>.
	/// </summary>
	/// <param name="g"><inheritdoc cref="RenderTo(Graphics)" path="/param[@name='g']"/></param>
	/// <seealso cref="View"/>
	private void DrawView(Graphics g)
	{
		if (View is null)
		{
			return;
		}

		DrawHouses(g);
		DrawCells(g);
		DrawCandidates(g);
		DrawLinks(g);
		DrawChute(g);
		DrawBabaGrouping(g);
		DrawFigure(g);
	}

	/// <summary>
	/// Draw eliminations.
	/// </summary>
	/// <param name="g"><inheritdoc cref="RenderTo(Graphics)" path="/param[@name='g']"/></param>
	private void DrawEliminations(Graphics g)
	{
		if (this is not
			{
				Conclusions: var conclusions and not [],
				Preferences: { EliminationColor: var eColor, CannibalismColor: var cColor },
				View: var view
			})
		{
			return;
		}

		using var elimBrush = new SolidBrush(eColor);
		using var cannibalBrush = new SolidBrush(cColor);
		using var elimBrushLighter = new SolidBrush(eColor.QuarterAlpha());
		using var cannibalismBrushLighter = new SolidBrush(cColor.QuarterAlpha());
		foreach (var (t, c, d) in conclusions)
		{
			if (t != Elimination)
			{
				continue;
			}

			var cannibalism = false;
			if (view is null)
			{
				goto Drawing;
			}

			foreach (var candidateNode in view.OfType<CandidateViewNode>())
			{
				var value = candidateNode.Candidate;
				if (value == c * 9 + d)
				{
					cannibalism = true;
					break;
				}
			}

		Drawing:
			g.FillEllipse(
				(cannibalism, view?.UnknownOverlaps(c)) switch
				{
					(true, true) => cannibalismBrushLighter,
					(true, false) => cannibalBrush,
					(false, true) => elimBrushLighter,
					_ => elimBrush
				},
				Calculator.GetMouseRectangle(c, d)
			);
		}
	}
}
