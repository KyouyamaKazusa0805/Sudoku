using System;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Manual;

namespace Sudoku.Solving
{
	/// <summary>
	/// Encapsulates all information after searched a solving step,
	/// which include the conclusion, the difficulty and so on.
	/// </summary>
	public abstract class TechniqueInfo : IEquatable<TechniqueInfo>
	{
		/// <summary>
		/// Provides passing data when initializing an instance of derived types.
		/// </summary>
		/// <param name="conclusions">The conclusions.</param>
		/// <param name="views">The views of this solving step.</param>
		protected TechniqueInfo(IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views) =>
			(Conclusions, Views) = (conclusions, views);


		/// <summary>
		/// <para>
		/// Indicates whether the difficulty rating of this technique should be
		/// shown in the output screen. Some techniques such as <b>Gurth's symmetrical placement</b>
		/// does not need to show the difficulty (because the difficulty of this technique
		/// is unstable).
		/// </para>
		/// <para>
		/// If the value is <see langword="true"/>, the analysis result will not show the difficulty
		/// of this instance.
		/// </para>
		/// </summary>
		public virtual bool ShowDifficulty => true;

		/// <summary>
		/// Indicates the technique name.
		/// </summary>
		public virtual string Name =>
			TechniqueCode.GetCustomName<TechniqueCode, TechniqueDisplayAttribute>("DisplayName") ?? string.Empty;

		/// <summary>
		/// The technique code of this instance used for comparison
		/// (e.g. search for specified puzzle that contains this technique).
		/// </summary>
		public virtual TechniqueCode TechniqueCode { get; }

		/// <summary>
		/// The difficulty or this step.
		/// </summary>
		public abstract decimal Difficulty { get; }

		/// <summary>
		/// The difficulty level of this step.
		/// </summary>
		public abstract DifficultyLevel DifficultyLevel { get; }

		/// <summary>
		/// All conclusions found in this technique step.
		/// </summary>
		public IReadOnlyList<Conclusion> Conclusions { get; }

		/// <summary>
		/// All views to display on the GUI.
		/// </summary>
		public IReadOnlyList<View> Views { get; }


		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="name">(<see langword="out"/> parameter) The name.</param>
		/// <param name="difficulty">(<see langword="out"/> parameter) The difficulty.</param>
		public void Deconstruct(out string name, out decimal difficulty) =>
			(name, difficulty) = (Name, Difficulty);

		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="name">(<see langword="out"/> parameter) The name.</param>
		/// <param name="difficulty">(<see langword="out"/> parameter) The difficulty.</param>
		/// <param name="difficultyLevel">(<see langword="out"/> parameter) The difficulty level.</param>
		public void Deconstruct(
			out string name, out decimal difficulty, out DifficultyLevel difficultyLevel) =>
			(name, difficulty, difficultyLevel) = (Name, Difficulty, DifficultyLevel);

		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="name">(<see langword="out"/> parameter) The name.</param>
		/// <param name="difficulty">(<see langword="out"/> parameter) The difficulty.</param>
		/// <param name="difficultyLevel">(<see langword="out"/> parameter) The difficulty level.</param>
		/// <param name="conclusions">(<see langword="out"/> parameter) All conclusions.</param>
		public void Deconstruct(
			out string name, out decimal difficulty, out DifficultyLevel difficultyLevel,
			out IReadOnlyList<Conclusion> conclusions) =>
			(name, difficulty, difficultyLevel, conclusions) = (Name, Difficulty, DifficultyLevel, Conclusions);

		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="name">(<see langword="out"/> parameter) The name.</param>
		/// <param name="difficulty">(<see langword="out"/> parameter) The difficulty.</param>
		/// <param name="difficultyLevel">(<see langword="out"/> parameter) The difficulty level.</param>
		/// <param name="conclusions">(<see langword="out"/> parameter) All conclusions.</param>
		/// <param name="views">(<see langword="out"/> parameter) All views.</param>
		public void Deconstruct(
			out string name, out decimal difficulty, out DifficultyLevel difficultyLevel,
			out IReadOnlyList<Conclusion> conclusions, out IReadOnlyList<View> views) =>
			(name, difficulty, difficultyLevel, conclusions, views) = (Name, Difficulty, DifficultyLevel, Conclusions, Views);

		/// <summary>
		/// Put this instance into the specified grid.
		/// </summary>
		/// <param name="grid">The grid.</param>
		public void ApplyTo(Grid grid)
		{
			foreach (var conclusion in Conclusions)
			{
				conclusion.ApplyTo(grid);
			}
		}

		/// <inheritdoc/>
		public override bool Equals(object? obj) =>
			obj is TechniqueInfo comparer && Equals(comparer);

		/// <inheritdoc/>
		public virtual bool Equals(TechniqueInfo other) =>
			ToString() == other.ToString();

		/// <inheritdoc/>
		public override int GetHashCode() => ToString().GetHashCode();

		/// <inheritdoc/>
		public abstract override string ToString();

		/// <summary>
		/// Returns a string that only contains the name and the conclusions.
		/// </summary>
		/// <returns>The string instance.</returns>
		public string ToSimpleString() =>
			$"{Name} => {new ConclusionCollection(Conclusions).ToString()}";


		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		public static bool operator ==(TechniqueInfo left, TechniqueInfo right) => left.Equals(right);

		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		public static bool operator !=(TechniqueInfo left, TechniqueInfo right) => !(left == right);
	}
}
