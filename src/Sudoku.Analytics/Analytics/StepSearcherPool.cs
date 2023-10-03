using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using Sudoku.Analytics.Metadata;

namespace Sudoku.Analytics;

/// <summary>
/// Represents a provider type that can find built-in <see cref="StepSearcher"/> instances.
/// </summary>
/// <seealso cref="StepSearcher"/>
public abstract class StepSearcherPool
{
	/// <summary>
	/// Indicates the current assembly.
	/// </summary>
	private static readonly Assembly ThisAssembly = typeof(StepSearcherPool).Assembly;


	/// <summary>
	/// Indicates an array of all built-in <see cref="StepSearcher"/>s that are defined in this assembly.
	/// </summary>
	/// <param name="expandSplitStepSearchers">
	/// Indicates whether we should expand split step searchers, creating multiple instances and inserting into the full collection.
	/// </param>
	/// <returns>An array of <see cref="StepSearcher"/> instances found.</returns>
	/// <seealso cref="SplitStepSearcherAttribute"/>
	[UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "<Pending>")]
	public static StepSearcher[] Default(bool expandSplitStepSearchers = true)
	{
		var result = new SortedList<int, StepSearcher>();
		foreach (var type in ThisAssembly.GetDerivedTypes(typeof(StepSearcher)))
		{
			if (!type.IsDefined(typeof(StepSearcherAttribute)))
			{
				continue;
			}

			if (!type.HasParameterlessConstructor())
			{
				continue;
			}

			foreach (var stepSearcher in GetStepSearchers(type, expandSplitStepSearchers))
			{
				result.Add(stepSearcher.Priority << 4 | stepSearcher.SplitPriority, stepSearcher);
			}
		}

		return [.. result.Values];
	}

	/// <summary>
	/// <inheritdoc cref="GetStepSearchers(Type, bool)" path="/summary"/>
	/// </summary>
	/// <param name="typeName">The raw type name. Please note that the string text shouldn't contain its containing namespace.</param>
	/// <param name="expandSplitStepSearchers">
	/// <inheritdoc cref="GetStepSearchers(Type, bool)" path="/param[@name='expandSplitStepSearchers']"/>
	/// </param>
	/// <returns><inheritdoc cref="GetStepSearchers(Type, bool)" path="/returns"/></returns>
	/// <exception cref="InvalidOperationException">
	/// Throws when the corresponding <see cref="Type"/> reflection result is not found.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[RequiresUnreferencedCode("Types might be removed.")]
	public static StepSearcher[] GetStepSearchers(string typeName, bool expandSplitStepSearchers)
		=> GetStepSearchers(ThisAssembly.GetType($"Sudoku.Analytics.StepSearchers.{typeName}")!, expandSplitStepSearchers);

	/// <summary>
	/// The internal method to get all <see cref="StepSearcher"/> instances derived from <paramref name="type"/> defined in this assembly.
	/// </summary>
	/// <param name="type">The type of the step searcher.</param>
	/// <param name="expandSplitStepSearchers">
	/// <inheritdoc cref="Default(bool)" path="/param[@name='expandSplitStepSearchers']"/>
	/// </param>
	/// <returns><inheritdoc cref="Default(bool)" path="/returns"/></returns>
	/// <seealso cref="SplitStepSearcherAttribute"/>
	public static StepSearcher[] GetStepSearchers(
		[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicProperties)] Type type,
		bool expandSplitStepSearchers
	)
	{
		// Check whether the step searcher is marked 'SeparatedAttribute'.
		switch ((SplitStepSearcherAttribute[])type.GetCustomAttributes<SplitStepSearcherAttribute>())
		{
			case { Length: var length and not 0 } splitAttributes when expandSplitStepSearchers:
			{
				var (i, stepSearcherArray) = (0, new StepSearcher[length]);

				// If the step searcher is marked 'SeparatedAttribute', we should sort them via priority at first.
				Array.Sort(splitAttributes, static (a, b) => a.Priority.CompareTo(b.Priority));

				// Then assign property values via reflection.
				foreach (var separatedAttribute in splitAttributes)
				{
					var instance = (StepSearcher)Activator.CreateInstance(type)!;
					foreach (var (name, value) in separatedAttribute.PropertyNamesAndValues.EnumerateAsPair<object, string, object>())
					{
						if (type.GetProperty(name) is { CanRead: true, CanWrite: true } propertyInfo)
						{
							// Assigns the property with attribute-configured value.
							// Please note that C# 9 feature "init-only" property is a compiler feature, rather than runtime one,
							// which means we can use reflection to set value to that property
							// no matter what the setter keyword is 'get' or 'init'.
							propertyInfo.SetValue(instance, value);
						}
					}

					// Sets the split priority value.
					// We should use reflection to set value because keyword used of the property is 'init', rather than 'set'.
					type.GetProperty(nameof(instance.SplitPriority))!.GetInitMethod(true)!.Invoke(instance, [separatedAttribute.Priority]);
					stepSearcherArray[i++] = instance;
				}

				return stepSearcherArray;
			}
			default:
			{
				return [(StepSearcher)Activator.CreateInstance(type)!];
			}
		}
	}
}
