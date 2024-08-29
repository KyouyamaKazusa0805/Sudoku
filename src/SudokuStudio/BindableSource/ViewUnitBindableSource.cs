namespace SudokuStudio.BindableSource;

/// <summary>
/// Represents a type that can be used for binding as source, for a view unit.
/// </summary>
public sealed partial class ViewUnitBindableSource : DependencyObject, ICloneable, IDrawable
{
	/// <summary>
	/// Initializes a <see cref="ViewUnitBindableSource"/> instance.
	/// </summary>
	public ViewUnitBindableSource() : this([], [])
	{
	}

	/// <summary>
	/// Initializes a <see cref="ViewUnitBindableSource"/> instance via the specified values.
	/// </summary>
	/// <param name="conclusions">The conclusions.</param>
	/// <param name="view">The view.</param>
	public ViewUnitBindableSource(Conclusion[] conclusions, View view) => (Conclusions, View) = (conclusions, view);


	/// <summary>
	/// Indicates the candidates as conclusions in a single <see cref="Step"/>.
	/// </summary>
	[DependencyProperty]
	public partial Conclusion[] Conclusions { get; set; }

	/// <summary>
	/// Indicates a view of highlight elements.
	/// </summary>
	[DependencyProperty]
	public partial View View { get; set; }

	/// <inheritdoc/>
	ReadOnlyMemory<Conclusion> IDrawable.Conclusions => Conclusions;

	/// <inheritdoc/>
	ReadOnlyMemory<View> IDrawable.Views => (View[])[View];


	/// <summary>
	/// Determine whether the collection contains the specified candidate.
	/// </summary>
	/// <param name="candidate">The candidate.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	public bool CandidateContains(Candidate candidate)
	{
		foreach (var conclusion in Conclusions)
		{
			if (conclusion.Candidate == candidate)
			{
				return true;
			}
		}
		foreach (var viewNode in View.OfType<CandidateViewNode>())
		{
			if (viewNode.Candidate == candidate)
			{
				return true;
			}
		}
		return false;
	}

	/// <inheritdoc cref="ICloneable.Clone"/>
	public ViewUnitBindableSource Clone() => new() { Conclusions = Conclusions[..], View = View.Clone() };

	/// <inheritdoc/>
	object ICloneable.Clone() => Clone();


	/// <summary>
	/// Make subtraction to get delta items.
	/// </summary>
	public static ViewUnitBindableSourceDiff operator -(ViewUnitBindableSource? old, ViewUnitBindableSource? @new)
	{
		return (old, @new) switch
		{
			(null, null) => new() { Negatives = [], Positives = [] },
			(not null, null) => new() { Negatives = f(old), Positives = [] },
			(null, not null) => new() { Negatives = [], Positives = f(@new) },
			_ => g(old, @new)
		};


		static ReadOnlySpan<IDrawableItem> f(ViewUnitBindableSource value)
			=> (IDrawableItem[])[.. value.Conclusions, .. value.View];

		static ViewUnitBindableSourceDiff g(ViewUnitBindableSource left, ViewUnitBindableSource right)
		{
			var (positives, negatives) = (new List<IDrawableItem>(), new List<IDrawableItem>());

			// TODO: Implement a way to control delta conclusions.
			negatives.AddRange(from conclusion in left.Conclusions select (IDrawableItem)conclusion);
			positives.AddRange(from conclusion in right.Conclusions select (IDrawableItem)conclusion);
			negatives.AddRange(from node in left.View.ExceptWith(right.View) select (IDrawableItem)node);
			positives.AddRange(from node in right.View.ExceptWith(left.View) select (IDrawableItem)node);
			return new() { Negatives = negatives.AsReadOnlySpan(), Positives = positives.AsReadOnlySpan() };
		}
	}
}
