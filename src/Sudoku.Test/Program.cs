using System.Linq;

string[] strings = { "1", "2", "3" };
_ = from @string in strings select (object)@string;
_ = from s in strings select (object)s;