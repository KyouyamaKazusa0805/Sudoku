using System.Runtime.CompilerServices;

namespace System.IO
{
	/// <summary>
	/// The directory extensions.
	/// </summary>
	public static class DirectoryEx
	{
		/// <summary>
		/// Create the specified directory path when the path doesn't exist.
		/// </summary>
		/// <param name="path">The path.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CreateIfDoesNotExist(string path)
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}

		/// <summary>
		/// Delete the directory when the current directory doesn't contain any files.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns>The <see cref="bool"/> result indicating whether the deletion is successful.</returns>
		public static bool DeleteWhenNoFilesInIt(string path)
		{
			if (Directory.GetFiles(path).Length == 0)
			{
				try
				{
					Directory.Delete(path);
					return true;
				}
				catch
				{
					return false;
				}
			}
			else
			{
				return false;
			}
		}
	}
}
