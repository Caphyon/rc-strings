using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace StringEnhancer
{
  public static class IDNormalizer
  {
    public static string NormalizeID(string aID)
    {
      if (aID.Length == 0)
        return Constants.kInvalidID;

      aID = aID.TrimEnd(new char[] { '\t', '/' });

      if (IsHexaRepresentation(aID)) // Hexa case
      {
        aID = Convert.ToInt32(aID, 16).ToString();
      }

      var canParseToInt = int.TryParse(aID, out var idInt);
      if (!canParseToInt || (idInt < 0 || idInt > Constants.kMaxID)) // ID is not in range [0, MaxID] or is invalid
        return Constants.kInvalidID;

      return aID;
    }

    public static string NormalizeReccurenceForID(string aID, Dictionary<string, string> aNameToID)
    {
      while (!Regex.IsMatch(aID, @"^\d+$") && aNameToID.ContainsKey(aID)) // Recurrent case
      {
        aID = aNameToID[aID];
      }

      return aID;
    }

    public static bool IsHexaRepresentation(string aString) =>
      aString.Length == 6 && aString.StartsWith("0x") && aString.Substring(2).All(IsHexaCharacter);

    public static bool IsHexaCharacter(char aChar) =>
      (aChar >= '0' && aChar <= '9') || (aChar >= 'A' && aChar <= 'F');
  }
}
