namespace SudokuStudio.Drawing;

/// <summary>
/// Extracted type that creates the capsule instances.
/// </summary>
/// <param name="pane">Indicates the sudoku pane control.</param>
/// <param name="converter">Indicates the position converter.</param>
internal sealed class GroupedNodeCreator(SudokuPane pane, SudokuPanePositionConverter converter) :
	CreatorBase<GroupedNodeInfo, Rectangle>(pane, converter)
{
	/// <inheritdoc/>
	public override ReadOnlySpan<Rectangle> CreateShapes(ReadOnlySpan<GroupedNodeInfo> nodes)
	{
		// Iterate on each inference to draw the links and grouped nodes (if so).
		var ((ow, _), _) = Converter;
		var drawnGroupedNodes = new List<CandidateMap>();
		var result = new List<Rectangle>();
		foreach (var n in nodes)
		{
			// If the start node or end node is a grouped node, we should append a rectangle to highlight it.
			var node = n.Map;
			if (node.Count != 1 && !drawnGroupedNodes.Contains(node))
			{
				drawnGroupedNodes.AddRef(in node);
				result.Add(drawRectangle(n, in node));
			}
		}
		return result.AsReadOnlySpan();


		Rectangle drawRectangle(GroupedNodeInfo n, ref readonly CandidateMap nodeCandidates)
		{
			var fill = new SolidColorBrush(Pane.GroupedNodeBackgroundColor);
			var stroke = new SolidColorBrush(Pane.GroupedNodeStrokeColor);
			var result = new Rectangle
			{
				Stroke = stroke,
				StrokeThickness = 1.5,
				Fill = fill,
				RadiusX = 10,
				RadiusY = 10,
				HorizontalAlignment = HorizontalAlignment.Left,
				VerticalAlignment = VerticalAlignment.Top,
				Tag = n,
				Opacity = Pane.EnableAnimationFeedback ? 0 : 1
			};

			// Try to arrange rectangle position.
			// A simple way is to record all rows and columns spanned for the candidate list,
			// in order to find four data:
			//   1) The minimal row
			//   2) The maximal row
			//   3) The minimal column
			//   4) The maximal column
			// and then find a minimal rectangle that can cover all of those candidates by those four data.
			const int logicalMaxValue = 100;
			var (minRow, minColumn, maxRow, maxColumn) = (Candidate.MaxValue, Candidate.MaxValue, Candidate.MinValue, Candidate.MinValue);
			var (minRowValue, minColumnValue, maxRowValue, maxColumnValue) = (logicalMaxValue, logicalMaxValue, -1, -1);
			foreach (var candidate in nodeCandidates)
			{
				var cell = candidate / 9;
				var digit = candidate % 9;
				var rowValue = cell / 9 * 3 + digit / 3;
				var columnValue = cell % 9 * 3 + digit % 3;
				if (rowValue <= minRowValue)
				{
					(minRowValue, minRow) = (rowValue, candidate);
				}
				if (rowValue >= maxRowValue)
				{
					(maxRowValue, maxRow) = (rowValue, candidate);
				}
				if (columnValue <= minColumnValue)
				{
					(minColumnValue, minColumn) = (columnValue, candidate);
				}
				if (columnValue >= maxColumnValue)
				{
					(maxColumnValue, maxColumn) = (columnValue, candidate);
				}
			}

			var topLeftY = Converter.GetPosition(minRow, Position.TopLeft).Y;
			var topLeftX = Converter.GetPosition(minColumn, Position.TopLeft).X;
			var bottomRightY = Converter.GetPosition(maxRow, Position.BottomRight).Y;
			var bottomRightX = Converter.GetPosition(maxColumn, Position.BottomRight).X;
			var rectanglePositionTopLeft = new Thickness(topLeftX - ow, topLeftY - ow, 0, 0);
			(result.Width, result.Height, result.Margin) = (bottomRightX - topLeftX, bottomRightY - topLeftY, rectanglePositionTopLeft);
			return result;
		}
	}
}
