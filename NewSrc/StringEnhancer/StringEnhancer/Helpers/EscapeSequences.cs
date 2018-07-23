using System;
using System.Collections.Generic;

namespace StringEnhancer
{
  public class EscapeSequences
  {
    #region Members

    private List<Tuple<string, string>> mResourceValueSequences = new List<Tuple<string, string>>
    {
      new Tuple<string, string>("\"\"", "\""),
      new Tuple<string, string>("&&", "&")
    };

    #endregion

    #region Public methods

    public string Escape(string aText)
    {
      foreach (var tuple in mResourceValueSequences)
        aText = aText.Replace(tuple.Item1, tuple.Item2);
      return aText;
    }

    public string Format(string aText)
    {
      foreach (var tuple in mResourceValueSequences)
        aText = aText.Replace(tuple.Item2, tuple.Item1);
      return aText;
    }

    #endregion
  }
}
