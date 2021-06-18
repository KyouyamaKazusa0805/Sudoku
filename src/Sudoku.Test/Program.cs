using System;

Console.WriteLine();


[AttributeUsage(AttributeTargets.All)]
internal class AAttribute : Attribute
{
	public int Property { get; set; }
}

internal sealed class AnotherAttribute : AAttribute
{

}