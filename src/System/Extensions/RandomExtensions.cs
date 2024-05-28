namespace System;

/// <summary>
/// Provides with extension methods on <see cref="Random"/>.
/// </summary>
/// <seealso cref="Random"/>
public static class RandomExtensions
{
	/// <summary>
	/// Generates a random number of type <typeparamref name="T"/>, obeying Gaussian's Normal Distribution,
	/// with σ value <paramref name="sigma"/> and μ value <paramref name="mu"/>.
	/// </summary>
	/// <typeparam name="T">The type of the result.</typeparam>
	/// <param name="this">The random number generator instance.</param>
	/// <param name="mu">Mu μ value.</param>
	/// <param name="sigma">Sigma σ value.</param>
	/// <returns>The result value.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T NextGaussian<T>(this Random @this, T mu, T sigma) where T : INumber<T>
	{
		var u1 = 1.0 - @this.NextDouble();
		var u2 = 1.0 - @this.NextDouble();
		var randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
		return mu + sigma * T.CreateChecked(randStdNormal);
	}
}
