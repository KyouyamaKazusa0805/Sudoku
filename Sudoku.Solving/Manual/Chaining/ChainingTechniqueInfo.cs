using System;
using System.Collections.Generic;
using System.Text;
using Sudoku.Constants;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using static Sudoku.Constants.Processings;
using static Sudoku.Constants.RegionLabel;
using static Sudoku.Solving.Manual.Chaining.Node.Cause;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Provides a usage of <b>chain</b> technique.
	/// </summary>
	public abstract class ChainingTechniqueInfo : TechniqueInfo, IEquatable<ChainingTechniqueInfo?>
	{
		/// <summary>
		/// Indicates whether the chain is a X-Chain.
		/// </summary>
		protected readonly bool _isX;

		/// <summary>
		/// Indicates whether the chain is a Y-Chain (multi-digit chain).
		/// </summary>
		/// <remarks>
		/// If <see cref="_isX"/> and <see cref="_isY"/> both <see langword="true"/>,
		/// the chain may be a XY-Chain (The chain consist of several bi-value cells).
		/// </remarks>
		/// <seealso cref="_isX"/>
		protected readonly bool _isY;

		/// <summary>
		/// Indicates whether the chain is nishio.
		/// </summary>
		protected readonly bool _isNishio;

		/// <summary>
		/// Indicates whether the chain is multiple.
		/// </summary>
		protected readonly bool _isMultiple;

		/// <summary>
		/// Indicates whether the chain is dynamic.
		/// </summary>
		protected readonly bool _isDynamic;

		/// <summary>
		/// Indicates the level.
		/// </summary>
		protected readonly int _level;


		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
		/// <param name="isX">Indicates whether the chain is a X-Chain.</param>
		/// <param name="isY">Indicates whether the chain is a Y-Chain.</param>
		protected ChainingTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, bool isX, bool isY)
			: base(conclusions, views) => (_isX, _isY) = (isX, isY);


		/// <summary>
		/// The flat complexity.
		/// </summary>
		public abstract int FlatComplexity { get; }

		/// <summary>
		/// Indicates the nested complexity.
		/// </summary>
		protected int NestedComplexity
		{
			get
			{
				int result = 0;
				var processed = new HashSet<FullChain>();
				foreach (var target in ChainsTargets)
				{
					foreach (var p in GetChain(target))
					{
						if (!(p.NestedChain is null))
						{
							var f = new FullChain(p.NestedChain);
							if (!processed.Contains(f))
							{
								result += p.NestedChain.NestedComplexity;
								processed.Add(f);
							}
						}
					}
				}

				return result;
			}
		}

		/// <summary>
		/// The sort key.
		/// </summary>
		public abstract int SortKey { get; }

		/// <summary>
		/// Indicates the cell.
		/// </summary>
		public int Cell
		{
			get
			{
				var result = Result;
				if (result?.IsOn ?? false)
				{
					return result.Candidate / 9;
				}

				return -1;
			}
		}

		/// <summary>
		/// Indicates the digit.
		/// </summary>
		public int Digit
		{
			get
			{
				var result = Result;
				if (result?.IsOn ?? false)
				{
					return result.Candidate % 9;
				}

				return -1;
			}
		}

		/// <summary>
		/// Indicates the total complexity.
		/// </summary>
		public int Complexity => FlatComplexity + NestedComplexity;

		/// <summary>
		/// The number of views.
		/// </summary>
		public int ViewCount => FlatViewCount + NestedViewCount;

		/// <summary>
		/// Indicates the flat view count.
		/// </summary>
		protected abstract int FlatViewCount { get; }

		/// <summary>
		/// Indicates the nested view count.
		/// </summary>
		protected int NestedViewCount
		{
			get
			{
				int result = 0;
				var processed = new HashSet<FullChain>();
				foreach (var target in ChainsTargets)
				{
					foreach (var p in GetChain(target))
					{
						if (!(p.NestedChain is null))
						{
							var f = new FullChain(p.NestedChain);
							if (!processed.Contains(f))
							{
								result += p.NestedChain.ViewCount;
								processed.Add(f);
							}
						}
					}
				}

				return result;
			}
		}

		/// <summary>
		/// The result.
		/// </summary>
		protected abstract Node? Result { get; }

		/// <summary>
		/// The chains targets.
		/// </summary>
		protected abstract ICollection<Node> ChainsTargets { get; }

		/// <summary>
		/// Indicates the name prefix.
		/// </summary>
		protected string NamePrefix =>
			_level > 0 ? "Dynamic " : ((_isNishio, _isMultiple, _isDynamic) switch
			{
				(true, _, _) => "Nishio ",
				(_, true, _) => string.Empty,
				(_, _, true) => "Dynamic ",
				_ => string.Empty
			});

		/// <summary>
		/// Indicates the name suffix.
		/// </summary>
		protected string NameSuffix => _level >= 1 ? $" Chains{GetNestedSuffix(_level)}" : " Chains";

		/// <summary>
		/// Indicates the common name.
		/// </summary>
		protected string? CommonName => !_isDynamic && !_isMultiple ? _isX ? "X-Chain" : "Y-Chain" : null;


		/// <summary>
		/// Get the chain using the specified target.
		/// </summary>
		/// <param name="target">The target node.</param>
		/// <returns>The chain.</returns>
		public static ICollection<Node> GetChain(Node target)
		{
			var result = new List<Node>();
			var done = new HashSet<Node>();
			var todo = new List<Node> { target };
			while (todo.Count != 0)
			{
				var next = new List<Node>();
				foreach (var p in todo)
				{
					if (!done.Contains(p))
					{
						done.Add(p);
						result.Add(p);
						next.AddRange(p.Parents);
					}
				}

				todo = next;
			}

			return result;
		}

		private (ChainingTechniqueInfo _chain, int _viewNumber)? GetNestedChain(int nestedViewNumber)
		{
			var processed = new HashSet<FullChain>();
			foreach (var target in ChainsTargets)
			{
				foreach (var p in GetChain(target))
				{
					if (!(p.NestedChain is null))
					{
						var f = new FullChain(p.NestedChain);
						if (!processed.Contains(f))
						{
							processed.Add(f);
							int localCount = p.NestedChain.ViewCount;
							if (localCount > nestedViewNumber)
							{
								return (p.NestedChain, nestedViewNumber);
							}

							nestedViewNumber -= localCount;
						}
					}
				}
			}

			return null;
		}

		private ICollection<ChainingTechniqueInfo> GetNestedChains()
		{
			var result = new List<ChainingTechniqueInfo>();
			var processed = new HashSet<FullChain>();
			foreach (var target in ChainsTargets)
			{
				foreach (var p in GetChain(target))
				{
					if (!(p.NestedChain is null))
					{
						var f = new FullChain(p.NestedChain);
						if (!processed.Contains(f))
						{
							result.Add(p.NestedChain);
							processed.Add(f);
						}
					}
				}
			}

			// Recursion (In case there is more than one level of nesting).
			foreach (var chain in new List<ChainingTechniqueInfo>(result))
			{
				result.AddRange(chain.GetNestedChains());
			}

			return result;
		}

		/// <summary>
		/// Get the end of the sub-chain from which the given nested chain can be eliminated.
		/// </summary>
		/// <param name="nestedChain">The nested chain.</param>
		/// <returns>
		/// The <see cref="Node"/> at which the nested chain starts.
		/// If the method cannot find the node, the return value will be <see langword="null"/>.
		/// </returns>
		private Node? GetContainerTarget(ChainingTechniqueInfo nestedChain)
		{
			foreach (var target in ChainsTargets)
			{
				foreach (var p in GetChain(target))
				{
					if (p.NestedChain == nestedChain)
					{
						return p;
					}
				}
			}

			return null;
		}

		/// <summary>
		/// Get color nodes.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <param name="state">The current state.</param>
		/// <param name="skipTarget">A <see cref="bool"/> value indicating whether should skip the target.</param>
		/// <returns>The map of all candidates.</returns>
		protected static SudokuMap GetColorNodes(Node target, bool state, bool skipTarget)
		{
			var result = new SudokuMap();
			foreach (var p in GetChain(target))
			{
				if (p.IsOn == state || state && (p != target || !skipTarget))
				{
					result.Add(p.Candidate);
				}
			}

			return result;
		}

		/// <summary>
		/// Get nested green nodes.
		/// </summary>
		/// <param name="nestedViewNumber">The current nested view number.</param>
		/// <returns>The map.</returns>
		protected SudokuMap GetNestedGreenNodes(int nestedViewNumber)
		{
			nestedViewNumber -= FlatViewCount;
			var (info, value) = GetNestedChain(nestedViewNumber)!.Value;
			return info.GetGreenNodes(value);
		}

		/// <summary>
		/// Get nested red nodes.
		/// </summary>
		/// <param name="nestedViewNumber">The current nested view number.</param>
		/// <returns>The map.</returns>
		protected SudokuMap GetNestedRedNodes(int nestedViewNumber)
		{
			nestedViewNumber -= FlatViewCount;
			var (info, value) = GetNestedChain(nestedViewNumber)!.Value;
			return info.GetRedNodes(value);
		}

		/// <summary>
		/// Get all blue nodes (it doesn't exist in the current view, but can exist in other views).
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="viewNumber">The current view number.</param>
		/// <returns>The map.</returns>
		protected SudokuMap GetBlueNodes(IReadOnlyGrid grid, int viewNumber)
		{
			// Create the grid eliminations from the container (or 'main') chain.
			var result = new SudokuMap();
			if ((viewNumber -= FlatViewCount) >= 0)
			{
				var nestedGrid = grid.Clone();
				var (nestedChain, nestedViewNumber) = GetNestedChain(viewNumber)!.Value;
				var target = GetContainerTarget(nestedChain)!;
				foreach (var p in GetChain(target))
				{
					if (!p.IsOn)
					{
						// Remove eliminations of the container chain.
						nestedGrid[p.Candidate / 9, p.Candidate % 9] = true;
					}
				}

				// Use the rule's parent collection.
				var blues = new List<Node>();
				var nestedTarget = nestedChain.GetChainTarget(nestedViewNumber);
				nestedChain.CollectRuleParents(grid, nestedGrid, blues, nestedTarget);

				// Convert to sudoku map.
				foreach (var p in blues)
				{
					// Get corresponding cell in initial grid.
					result.Add(p.Candidate);
				}
			}

			return result;
		}

		/// <summary>
		/// Get all nested links.
		/// </summary>
		/// <param name="nestedViewNumber">The current view number.</param>
		/// <returns>The links.</returns>
		protected ICollection<Link> GetNestedLinks(int nestedViewNumber)
		{
			nestedViewNumber -= FlatViewCount;
			var (info, value) = GetNestedChain(nestedViewNumber)!.Value;
			return info.GetNestedLinks(value);
		}

		/// <summary>
		/// Get all links according to the target node.
		/// </summary>
		/// <param name="target">The target node.</param>
		/// <returns>All links.</returns>
		protected static ICollection<Link> GetLinks(Node target)
		{
			var result = new List<Link>();
			foreach (var p in GetChain(target))
			{
				if (p.Parents.Count <= 6)
				{
					// Add links from all parents of p to p.
					foreach (var pr in p.Parents)
					{
						result.Add(new Link(pr.Candidate, p.Candidate, LinkType.Default));
					}
				}
			}

			return result;
		}


		private static RegionLabel? GetCauseRegion(Node.Cause cause)
		{
			switch (cause)
			{
				case HiddenSingleRow:
				{
					return Row;
				}
				case HiddenSingleColumn:
				{
					return Column;
				}
				case HiddenSingleBlock:
				{
					return Block;
				}
				default:
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Get rule parents.
		/// </summary>
		/// <param name="initialGrid">The initial grid.</param>
		/// <param name="currentGrid">The current grid.</param>
		/// <returns>All nodes.</returns>
		public ICollection<Node> GetRuleParents(IReadOnlyGrid initialGrid, Grid currentGrid)
		{
			var result = new HashSet<Node>();
			// Warning: Iterate on each chain target separately. Reason: They may be equal
			// according to Equals() (same candidate), but they may have different parents!
			foreach (var target in ChainsTargets)
			{
				CollectRuleParents(initialGrid, currentGrid, result, target);
			}

			return result;
		}

		/// <summary>
		/// Collect all rule parents.
		/// </summary>
		/// <param name="initialGrid">The initial grid.</param>
		/// <param name="currentGrid">The current grid.</param>
		/// <param name="result">The result.</param>
		/// <param name="target">The target.</param>
		protected void CollectRuleParents(
			IReadOnlyGrid initialGrid, Grid currentGrid, ICollection<Node> result, Node target)
		{
			var done = new HashSet<Node>();
			var todo = new List<Node> { target };
			while (todo.Count != 0)
			{
				var next = new List<Node>();
				foreach (var p in todo)
				{
					if (!done.Contains(p))
					{
						done.Add(p);
						var cause = p.NodeCause;
						if (!cause.HasValue)
						{
							cause = this switch
							{
								CellChainingTechniqueInfo _ => NakedSingle,
								RegionChainingTechniqueInfo info =>
									ChainingTechniqueSearcher.GetRegionCause(info.GetRegionLabel()),
								_ => cause
							};
						}

						if (p.IsOn && cause.HasValue)
						{
							int cell = p.Candidate / 9;
							if (cause.Value == NakedSingle)
							{
								short actCellMask = currentGrid.GetCandidateMask(cell);
								short initCellMask = initialGrid.GetCandidateMask(cell);
								for (int digit = 0; digit < 9; digit++)
								{
									if (currentGrid.Exists(cell, digit) is true && initialGrid.Exists(cell, digit) is true)
									{
										result.Add(new Node(cell * 9 + digit, false));
									}
								}
							}
							else // Hidden single.
							{
								var map = RegionMaps[
									GetCauseRegion(cause.Value) switch
									{
										Block => GetRegion(cell, Block),
										Column => GetRegion(cell, Column),
										_ => GetRegion(cell, Row),
									}];
								foreach (int currentCell in map)
								{
									int digit = p.Candidate % 9;
									if (currentGrid.Exists(currentCell, digit) is true
										&& initialGrid.Exists(currentCell, digit) is true)
									{
										result.Add(new Node(currentCell * 9 + digit, false));
									}
								}
							}
						}

						next.AddRange(p.Parents);
					}
				}

				todo = next;
			}
		}

		/// <summary>
		/// Get the chain target according to the specified view number.
		/// </summary>
		/// <param name="viewNumber">The view number.</param>
		/// <returns>The target node in the current view.</returns>
		protected abstract Node GetChainTarget(int viewNumber);

		/// <summary>
		/// Get the number of the ancestors of the specified child node.
		/// </summary>
		/// <param name="child">The child node.</param>
		/// <returns>The number of its ancestors.</returns>
		protected static int GetAncestorCount(Node child)
		{
			var ancestors = new HashSet<Node>();
			var todo = new List<Node> { child };
			while (todo.Count != 0)
			{
				var next = new List<Node>();
				foreach (var p in todo)
				{
					if (!ancestors.Contains(p))
					{
						ancestors.Add(p);
						next.AddRange(p.Parents);
					}
				}

				todo = next;
			}

			return ancestors.Count;
		}

		/// <summary>
		/// Get nested suffix using the specified level.
		/// </summary>
		/// <param name="level">The level.</param>
		/// <returns>The suffix.</returns>
		protected string GetNestedSuffix(int level) =>
			level switch
			{
				1 => " (+)",
				2 => " (+ Forcing Chains)",
				3 => " (+ Multiple Forcing Chains)",
				4 => " (+ Dynamic Forcing Chains)",
				_ when level >= 5 => $" (+ Dynamic Forcing Chains{GetNestedSuffix(level - 3)})",
				_ => string.Empty
			};

		/// <summary>
		/// Add the chain item.
		/// </summary>
		/// <param name="nodes">The nodes.</param>
		/// <param name="rules">The rules.</param>
		/// <param name="p">The node.</param>
		private void AddChainItem(IList<Node> nodes, IList<string> rules, Node p)
		{
			// First add parent chains.
			foreach (var parent in p.Parents)
			{
				AddChainItem(nodes, rules, parent);
			}

			var rule = new StringBuilder($"({rules.Count + 1}) If ");
			for (int count = p.Parents.Count - 1, i = count; i >= 0; i--)
			{
				if (i < count)
				{
					rule.Append(i == 0 ? " and " : ", ");
				}

				var parent = p.Parents[i];
				rule.Append(parent.ToString(LinkType.Weak));

				int pIndex = nodes.IndexOf(parent);
				if (pIndex < rules.Count - 1)
				{
					rule.Append($"({(pIndex >= 0 ? $"{pIndex + 1}" : "initial assumption")})");
				}

				rule.Append(", then ").Append(p.ToString(LinkType.Strong));
				if (!(p.Explanation is null))
				{
					rule.Append($" ({p.Explanation})");
				}

				nodes.Add(p);
				rules.Add(rule.ToString());
			}
		}

		/// <summary>
		/// Get the source node.
		/// </summary>
		/// <param name="target">The target node.</param>
		/// <returns>The source node.</returns>
		protected static Node GetSourceNode(Node target)
		{
			var result = target;
			while (result.Parents.Count != 0)
			{
				result = result.Parents[0];
			}

			return result;
		}

		/// <summary>
		/// Get the chain string.
		/// </summary>
		/// <param name="dest">The destination node.</param>
		/// <returns>The string.</returns>
		protected string GetHtmlChain(Node dest)
		{
			var nodes = new List<Node>();
			var rules = new List<string>();
			AddChainItem(nodes, rules, dest);

			var result = new StringBuilder();
			foreach (string rule in rules)
			{
				result.AppendLine(rule);
			}

			return result.ToString();
		}

		/// <summary>
		/// Append the nested chain details string.
		/// </summary>
		/// <param name="result">The current string.</param>
		/// <returns>The result string.</returns>
		public string AppendNestedChainDetails(string result)
		{
			var nestedChains = GetNestedChains();
			if (nestedChains.Count == 0)
			{
				return result;
			}

			var nested = new StringBuilder()
				.AppendLine()
				.AppendLine()
				.Append("Nested Forcing Chains details ")
				.AppendLine(
					"(Note that each Nested Forcing Chains relies on the fact that some blue candidates have been" +
					"eliminated by the main Forcing Chains):")
				.AppendLine();
			int index = FlatViewCount + 1;
			foreach (var nestedHint in nestedChains)
			{
				nested.Append("Nested ").Append(nestedHint.ToString()).AppendLine();
				foreach (var target in nestedHint.ChainsTargets)
				{
					var assumption = GetSourceNode(target);
					nested
						.Append($"Chain ")
						.Append(index)
						.Append(": If ")
						.Append(assumption.ToString(LinkType.Weak))
						.Append(", then")
						.Append(target.ToString(LinkType.Strong))
						.Append(" (View")
						.Append(index)
						.AppendLine("):")
						.Append(GetHtmlChain(target))
						.AppendLine();

					index++;
				}
			}

			int pos = result.ToLower().IndexOf("</body>");
			return $"{result[0..pos]}{nested}{pos..}";
		}

		/// <inheritdoc/>
		public sealed override bool Equals(object? obj) => Equals(obj as ChainingTechniqueInfo);

		/// <inheritdoc/>
		public sealed override bool Equals(TechniqueInfo? other) => Equals(other as ChainingTechniqueInfo);

		/// <inheritdoc/>
		public bool Equals(ChainingTechniqueInfo? other) => Equals(this, other);

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			var result = new HashCode();
			foreach (var conclusion in Conclusions)
			{
				result.Add(conclusion);
			}

			return result.ToHashCode();
		}

		/// <summary>
		/// To determine whether two <see cref="ChainingTechniqueInfo"/>s are equal.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns>A <see cref="bool"/> value.</returns>
		private static bool Equals(ChainingTechniqueInfo? left, ChainingTechniqueInfo? right) =>
			(left is null, right is null) switch
			{
				(true, true) => true,
				(false, false) =>
					new ConclusionCollection(left!.Conclusions) == new ConclusionCollection(right!.Conclusions),
				_ => false
			};


		/// <include file='../../../GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		public static bool operator ==(ChainingTechniqueInfo? left, ChainingTechniqueInfo? right) => Equals(left, right);

		/// <include file='../../../GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		public static bool operator !=(ChainingTechniqueInfo? left, ChainingTechniqueInfo? right) => !(left == right);
	}
}
