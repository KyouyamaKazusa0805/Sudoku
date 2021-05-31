using System;

var s = new S(1, 2, new(1, 2, null));

if (s is { Property1: >= 1, Property2: 20D, NestedMember: null, Property1: < 100 })
	Console.WriteLine(s); // SS9005.

record S(int Property1, double Property2, S? NestedMember);