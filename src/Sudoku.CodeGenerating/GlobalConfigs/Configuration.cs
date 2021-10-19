namespace Sudoku.CodeGenerating.GlobalConfigs;

/// <summary>
/// Encapsulates the methods that checks the configuration attributes in these current source generator types.
/// </summary>
internal static class Configuration
{
	/// <summary>
	/// Determines whether the specified source generator is marked <see cref="EmitNullableEnableAttribute"/>.
	/// </summary>
	/// <param name="type">The source genrator type.</param>
	/// <returns>The <see cref="bool"/> result.</returns>
	public static bool EmitsNullableEnable(Type type) =>
		type.GetInterfaces() is var interfaces
		&& (typeof(ISourceGenerator), typeof(IIncrementalGenerator)) is (var sg, var ig)
		&& Array.Exists(interfaces, i => i == sg || i == ig)
			? type.IsDefined(typeof(EmitNullableEnableAttribute))
			: throw new ArgumentException(
				$@"The specified type argument must implement the interface '{
					nameof(ISourceGenerator)
				}' or '{
					nameof(IIncrementalGenerator)
				}'."
			);

	/// <summary>
	/// Determines whether the specified source generator is marked <see cref="EmitNullableEnableAttribute"/>.
	/// </summary>
	/// <typeparam name="T">The source generator type.</typeparam>
	/// <returns>The <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool EmitsNullableEnable<T>() => EmitsNullableEnable(typeof(T));

	/// <summary>
	/// Gets the <see cref="string"/> representation of the <c>#nullable enable</c>
	/// if the source generator type allows emitting the <c>#nullable</c> directive.
	/// </summary>
	/// <param name="type">The source generator type.</param>
	/// <returns>The <see cref="string"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static string GetNullableEnableString(Type type) =>
		EmitsNullableEnable(type) ? "#nullable enable" : "// Configuration is disabled the nullable settings.";
}
