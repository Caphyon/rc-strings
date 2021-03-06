﻿using System;
using System.Linq;

namespace Caphyon.RcStrings.StringEnhancer
{
  public class StringLine
  {
    #region Members

    private string mValue;
    private string mLine;

    #endregion

    #region Properties

    public int Id { get; internal set; }
    public int  RcOrder { get; internal set; }
    public string Name { get; private set; }

    public string Value
    {
      get => mValue;
      set
      {
        mValue = string.Format("\"{0}\"", new EscapeSequences().Format(mValue));
        mLine = mLine != null ? mLine.Replace(mValue, string.Format("\"{0}\"", value)) 
          : GenerateLine(Name, mValue);
        mValue = value;
      }
    }
    #endregion

    #region Ctor

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

      if (!String.IsNullOrEmpty(aValue) && aValue.First() == '\"')
        aValue = aValue.Remove(0, 1);
      if (!String.IsNullOrEmpty(aValue) && aValue.Last() == '\"')
        aValue = aValue.Remove(aValue.Length - 1, 1);

      mValue = aValue;
      mLine = aLine != null ? aLine : GenerateLine(aName, aValue);
    }

    #endregion

    #region Public methods

    public override string ToString() => mLine;

    #endregion

    #region Private methods

    private string GenerateLine(string aName, string aValue)
    {
      string spacesFromBegin = "    ";
      string spacesAfterName = "                            ";

      if (aName.Length <= ParseConstants.kMaximumLengthForMoreSpacesAfterName)
      {
        spacesAfterName = spacesAfterName.Remove(0, aName.Length + spacesFromBegin.Length);
        return string.Format("{0}{1}{2}\"{3}\"", spacesFromBegin, aName, spacesAfterName, aValue);
      }
      else if (aName.Length <= ParseConstants.kMaximumLengthToWriteASingleLine)
        return string.Format("{0}{1} \"{2}\"", spacesFromBegin, aName, aValue);

      return string.Format("{0}{1} \r\n{2}\"{3}\"", spacesFromBegin, aName, spacesAfterName, aValue);
    }

    #endregion
  }
}
