using System;

int? p = 30;
int q = 40;

int? r = p is null ? q : p;
Console.WriteLine(r);