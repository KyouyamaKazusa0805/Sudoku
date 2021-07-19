using System.Text;

ValueStringBuilder sb = new(stackalloc char[10]);
sb.Append("Hello");
sb.Append('!');
sb.Dispose();