using System.Collections.Generic;

namespace StringEnhancer
{
  public class HeaderId
  {
    private string mValue;

    public HeaderId(string aValue)
    {
      Value = aValue;
    }

    public HeaderId(HeaderId aHeaderId)
    {
      Value = aHeaderId.Value;
      IsHexa = aHeaderId.IsHexa;
    }

    public string Value
    {
      get => mValue;
      set
      {
        IsHexa = IDValidator.IsHexaRepresentation(value);
        mValue = value;
      }
    }

    public bool IsHexa { get; set; }
  }
}