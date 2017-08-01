using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caphyon.RcStrings.StringEnhancer
{
  public class StringLine
  {
    public int Id { get; internal set; }
    public int  RcOrder { get; internal set; }
    public string Name { get; private set; }

    private string mValue;
    private string mLine;

    public StringLine(string aName, string aValue, int aId)
    {
      Name = aName;
      mValue = aValue;
      Id = aId;
      mLine = GenerateLine(Name, mValue);
    }

    public StringLine(string aName, string aValue, string aLine = null)
    {
      RcOrder = ParseConstants.kRcOrderDefaultValue;
      Id = ParseConstants.kIdDefaultValue;
      Name = aName;

      mValue = aValue.Trim('"');
      mLine = aLine != null ? aLine : GenerateLine(aName, aValue);
    }

    public override string ToString() => mLine;

    public string Value
    {
      get => mValue;
      set
      {
        mValue = new EscapeCharacters().Format(mValue);
        mLine = mLine != null ? mLine.Replace(mValue, value) : 
          GenerateLine(Name, mValue);
        mValue = value;
      }
    }

    private string GenerateLine(string aName, string aValue)
    {
      string spacesFromBegin = "    ";
      string spacesAfterName = "                            ";

      //23 is the maximum length to write on a single line 
      if (aName.Length <= ParseConstants.kMaximumLengthToWriteASingleLine)
      {
        spacesAfterName = spacesAfterName.Remove(0, aName.Length + spacesFromBegin.Length);
        return string.Format("{0}{1}{2}\"{3}\"", spacesFromBegin, aName, spacesAfterName, aValue);
      }
      return string.Format("{0}{1} \r\n{2}\"{3}\"", spacesFromBegin, aName, spacesAfterName, aValue);
    }
  }
}
