using System.Diagnostics.CodeAnalysis;

namespace System
{
	public interface ICloneable<out T> : ICloneable where T : class
	{
		[return: NotNull]
		new T Clone();

		object ICloneable.Clone() => Clone();
	}
}
