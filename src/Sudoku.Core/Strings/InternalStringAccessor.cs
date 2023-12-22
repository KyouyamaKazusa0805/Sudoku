using System.Globalization;
using System.Runtime.CompilerServices;

namespace Sudoku.Strings;

internal static class InternalStringAccessor
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string GetString(string key) => InternalResources.ResourceManager.GetString(key)!;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string GetString(string key, CultureInfo? culture) => InternalResources.ResourceManager.GetString(key, culture)!;
}
