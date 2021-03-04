using System;
using Sudoku.Data;
using Sudoku.DocComments;

namespace Sudoku.Solving.Manual.Exocets
{
	/// <summary>
	/// Indicates an exocet pattern.
	/// </summary>
	public readonly struct Pattern : IValueEquatable<Pattern>
	{
		/// <summary>
		/// Initializes an instance with the specified cells.
		/// </summary>
		/// <param name="base1">The base cell 1.</param>
		/// <param name="base2">The base cell 2.</param>
		/// <param name="tq1">The target Q1 cell.</param>
		/// <param name="tq2">The target Q2 cell.</param>
		/// <param name="tr1">The target R1 cell.</param>
		/// <param name="tr2">The target R2 cell.</param>
		/// <param name="crossline">(<see langword="in"/> parameter) The cross line cells.</param>
		/// <param name="mq1">(<see langword="in"/> parameter) The mirror Q1 cell.</param>
		/// <param name="mq2">(<see langword="in"/> parameter) The mirror Q2 cell.</param>
		/// <param name="mr1">(<see langword="in"/> parameter) The mirror R1 cell.</param>
		/// <param name="mr2">(<see langword="in"/> parameter) The mirror R2 cell.</param>
		public Pattern(
			int base1, int base2, int tq1, int tq2, int tr1, int tr2, in Cells crossline,
			in Cells mq1, in Cells mq2, in Cells mr1, in Cells mr2)
		{
			CrossLine = crossline;
			Base1 = base1;
			Base2 = base2;
			TargetQ1 = tq1;
			TargetQ2 = tq2;
			TargetR1 = tr1;
			TargetR2 = tr2;
			MirrorQ1 = mq1;
			MirrorQ2 = mq2;
			MirrorR1 = mr1;
			MirrorR2 = mr2;
		}


		/// <summary>
		/// Indicates the base cell 1.
		/// </summary>
		public int Base1 { get; }

		/// <summary>
		/// Indicates the base cell 2.
		/// </summary>
		public int Base2 { get; }

		/// <summary>
		/// Indicates the target Q1 cell.
		/// </summary>
		public int TargetQ1 { get; }

		/// <summary>
		/// Indicates the target Q2 cell.
		/// </summary>
		public int TargetQ2 { get; }

		/// <summary>
		/// Indicates the target R1 cell.
		/// </summary>
		public int TargetR1 { get; }

		/// <summary>
		/// Indicates the target R2 cell.
		/// </summary>
		public int TargetR2 { get; }

		/// <summary>
		/// Indicates the cross line cells.
		/// </summary>
		public Cells CrossLine { get; }

		/// <summary>
		/// Indicates the mirror Q1 cell.
		/// </summary>
		public Cells MirrorQ1 { get; }

		/// <summary>
		/// Indicates the mirror Q2 cell.
		/// </summary>
		public Cells MirrorQ2 { get; }

		/// <summary>
		/// Indicates the mirror R1 cell.
		/// </summary>
		public Cells MirrorR1 { get; }

		/// <summary>
		/// Indicates the mirror R2 cell.
		/// </summary>
		public Cells MirrorR2 { get; }


		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="baseCellsMap">(<see langword="out"/> parameter) The base cells.</param>
		/// <param name="targetCellsMap">(<see langword="out"/> parameter) The target cells.</param>
		/// <param name="crosslineMap">(<see langword="out"/> parameter) The cross-line cells.</param>
		public void Deconstruct(out Cells baseCellsMap, out Cells targetCellsMap, out Cells crosslineMap)
		{
			baseCellsMap = new() { Base1, Base2 };
			targetCellsMap = new() { TargetQ1, TargetQ2, TargetR1, TargetR2 };
			crosslineMap = CrossLine;
		}

		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="base1">(<see langword="out"/> parameter) The base cell 1.</param>
		/// <param name="base2">(<see langword="out"/> parameter) The base cell 2.</param>
		/// <param name="tq1">(<see langword="out"/> parameter) The target Q1 cell.</param>
		/// <param name="tq2">(<see langword="out"/> parameter) The target Q2 cell.</param>
		/// <param name="tr1">(<see langword="out"/> parameter) The target R1 cell.</param>
		/// <param name="tr2">(<see langword="out"/> parameter) The target R2 cell.</param>
		public void Deconstruct(out int base1, out int base2, out int tq1, out int tq2, out int tr1, out int tr2)
		{
			base1 = Base1;
			base2 = Base2;
			tq1 = TargetQ1;
			tq2 = TargetQ2;
			tr1 = TargetR1;
			tr2 = TargetR2;
		}

		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="base1">(<see langword="out"/> parameter) The base cell 1.</param>
		/// <param name="base2">(<see langword="out"/> parameter) The base cell 2.</param>
		/// <param name="tq1">(<see langword="out"/> parameter) The target Q1 cell.</param>
		/// <param name="tq2">(<see langword="out"/> parameter) The target Q2 cell.</param>
		/// <param name="tr1">(<see langword="out"/> parameter) The target R1 cell.</param>
		/// <param name="tr2">(<see langword="out"/> parameter) The target R2 cell.</param>
		/// <param name="crossline">(<see langword="out"/> parameter) The cross line cells.</param>
		/// <param name="mq1">(<see langword="out"/> parameter) The mirror Q1 cell.</param>
		/// <param name="mq2">(<see langword="out"/> parameter) The mirror Q2 cell.</param>
		/// <param name="mr1">(<see langword="out"/> parameter) The mirror R1 cell.</param>
		/// <param name="mr2">(<see langword="out"/> parameter) The mirror R2 cell.</param>
		/// <param name="baseCellsMap">(<see langword="out"/> parameter) The base cells.</param>
		/// <param name="targetCellsMap">(<see langword="out"/> parameter) The target cells.</param>
		/// <param name="crosslineMap">(<see langword="out"/> parameter) The cross-line cells.</param>
		public void Deconstruct(
			out int base1, out int base2, out int tq1, out int tq2, out int tr1, out int tr2,
			out Cells crossline, out Cells mq1, out Cells mq2, out Cells mr1, out Cells mr2,
			out Cells baseCellsMap, out Cells targetCellsMap, out Cells crosslineMap)
		{
			base1 = Base1;
			base2 = Base2;
			tq1 = TargetQ1;
			tq2 = TargetQ2;
			tr1 = TargetR1;
			tr2 = TargetR2;
			crossline = CrossLine;
			mq1 = MirrorQ1;
			mq2 = MirrorQ2;
			mr1 = MirrorR1;
			mr2 = MirrorR2;
			baseCellsMap = new() { Base1, Base2 };
			targetCellsMap = new() { TargetQ1, TargetQ2, TargetR1, TargetR2 };
			crosslineMap = CrossLine;
		}

		/// <inheritdoc cref="object.Equals(object?)"/>
		public override bool Equals(object? obj) => obj is Pattern comparer && Equals(comparer);

		/// <inheritdoc/>
		[CLSCompliant(false)]
		public bool Equals(in Pattern other) =>
			Base1 == other.Base1 && Base2 == other.Base2
			&& TargetQ1 == other.TargetQ1 && TargetQ2 == other.TargetQ2
			&& TargetR1 == other.TargetR1 && TargetR2 == other.TargetR2
			&& MirrorQ1 == other.MirrorQ1 && MirrorQ2 == other.MirrorQ2
			&& MirrorR1 == other.MirrorR1 && MirrorR2 == other.MirrorR2
			&& CrossLine == other.CrossLine;

		/// <inheritdoc cref="object.GetHashCode"/>
		public override int GetHashCode()
		{
			var hashCode = new HashCode();
			hashCode.Add(Base1);
			hashCode.Add(Base2);
			hashCode.Add(TargetQ1);
			hashCode.Add(TargetQ2);
			hashCode.Add(MirrorR1);
			hashCode.Add(MirrorR2);
			hashCode.Add(MirrorQ1);
			hashCode.Add(MirrorQ2);
			hashCode.Add(MirrorR1);
			hashCode.Add(MirrorR2);
			hashCode.Add(CrossLine);
			return hashCode.ToHashCode();
		}

		/// <inheritdoc cref="object.ToString"/>
		public override string ToString()
		{
			string baseCellsStr = new Cells { Base1, Base2 }.ToString();
			string targetCellsStr = new Cells { TargetQ1, TargetQ2, TargetR1, TargetR2 }.ToString();
			return $"Exocet: base {baseCellsStr}, target {targetCellsStr}";
		}


		/// <inheritdoc cref="Operators.operator =="/>
		public static bool operator ==(in Pattern left, in Pattern right) => left.Equals(right);

		/// <inheritdoc cref="Operators.operator !="/>
		public static bool operator !=(in Pattern left, in Pattern right) => !(left == right);
	}
}
