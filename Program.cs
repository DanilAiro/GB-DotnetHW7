using System.Reflection;
using System.Text;

namespace HW7;

internal class Program
{
  static void Main(string[] args)
  {
    TestClass obj = new TestClass(1, "ку", 0.5m, ['f', 'g', 'h'] );

    Console.WriteLine(GetEInfo(obj));

    Console.WriteLine(GetObjFromString(GetEInfo(obj)));
  }

  static string GetEInfo(object obj)
  {
    if (obj == null)
    {
      return String.Empty;
    }

    Type type = obj.GetType();
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append(type.FullName);
    stringBuilder.Append("\n");

    foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
    {
      var fieldValue = field.GetValue(obj);
      if (fieldValue is char[])
      {
        stringBuilder.Append(field.Name + ":::" + "char[]");

        foreach (var charValue in ((char[])fieldValue))
        {
          stringBuilder.Append(charValue);
        }

        stringBuilder.Append('\n');
      }
      else
      {
        stringBuilder.Append(field.Name + ":::" + type.GetField(field.Name, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj));
        stringBuilder.Append('\n');
      }

    }

    return stringBuilder.ToString();
  }

  static object GetObjFromString(string info)
  {
    if (info == String.Empty)
    {
      return new object();
    }

    string[] lines = info.Split('\n');
    Type type = Type.GetType(lines[0]);
    var temp = Activator.CreateInstance(type);

    foreach (var line in lines)
    {
      if (line.Contains(":::"))
      {
        var map = line.Split(":::");
        var field = type.GetField(map[0], BindingFlags.NonPublic | BindingFlags.Instance);

        if (int.TryParse(map[1], out int i))
        {
          field?.SetValue(temp, i);
        }
        else if (decimal.TryParse(map[1], out decimal d))
        {
          field?.SetValue(temp, d);
        }
        else if (map[1].StartsWith("char[]"))
        {
          field?.SetValue(temp, map[1].Replace("char[]", "").ToCharArray());
        }
        else
        {
          field?.SetValue(temp, map[1]);
        }
      }
    }

    return temp;
  }
}

class TestClass
{
  public int I { get; set; }
  public string? S { get; set; }
  public decimal D { get; set; }
  public char[]? C { get; set; }

  public TestClass()
  { }
  private TestClass(int i)
  {
    this.I = i;
  }
  public TestClass(int i, string s, decimal d, char[] c) : this(i)
  {
    this.S = s;
    this.D = d;
    this.C = c;
  }

  public override string ToString()
  {
    return $"{I} + {S} + {D} + [{C[0]}, {C[1]}, {C[2]}]";
  }
}
