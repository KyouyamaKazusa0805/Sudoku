using System;

var s = new S(1, 2, new(1, 2, null));

Console.WriteLine(s);

if (s is { Property1: 1 } and { Property2: 20 } and { NestedMember: null })
	Console.WriteLine(s); // SS9005.
if (s is { Property1: 1 } or { Property2: 20 } or { NestedMember: null })
	Console.WriteLine(s); // SS9005.
if (s is { Property1: 1 } and { Property2: 20 } or { NestedMember: null })
	Console.WriteLine(s); // SS9005.
if (s is ({ Property1: 1 } or { Property2: 20 }) and { NestedMember: null })
	Console.WriteLine(s); // SS9005.

record S(int Property1, double Property2, S? NestedMember);