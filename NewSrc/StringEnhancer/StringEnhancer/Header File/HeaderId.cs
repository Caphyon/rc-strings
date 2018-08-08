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

    #region Operator Overloading and overriding Equals() and GetHashCode()
    public override bool Equals(object obj)
    {
      var id = obj as HeaderId;
      return id != null &&
             mValue == id.mValue &&
             Value == id.Value &&
             IsHexa == id.IsHexa;
    }

    public override int GetHashCode()
    {
      var hashCode = -409543468;
      hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(mValue);
      hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Value);
      hashCode = hashCode * -1521134295 + IsHexa.GetHashCode();
      return hashCode;
    }

    public static bool operator ==(HeaderId first, HeaderId second) =>
      first?.Value == second?.Value;
    public static bool operator !=(HeaderId first, HeaderId second) =>
      first?.Value != second?.Value;
    #endregion
  }
}