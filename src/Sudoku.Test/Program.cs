using System.Text;

var vsb = new ValueStringBuilder(new char[50]);
ValueStringBuilder vsb2 = new(new char[50]);
var vsb3 = new ValueStringBuilder(new[] { '\0', '\0', '\0', '\0', '\0' });
ValueStringBuilder vsb4 = new(new[] { '\0', '\0', '\0', '\0', '\0' });