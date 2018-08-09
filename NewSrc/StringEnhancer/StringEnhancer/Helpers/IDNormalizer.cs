using System;

namespace StringEnhancer
{
  public static class IDNormalizer
  {
    public static HeaderId NormalizeHexaID(HeaderId aID)
    {
      if (!IDValidator.IsInValidRange(aID))
        aID.Value = Constants.kInvalidID.Value;

      if (IDValidator.IsHexaRepresentation(aID))
        aID.Value = Convert.ToInt32(aID.Value, 16).ToString();

      if (!IDValidator.IsValidInteger(aID))
        aID.Value = Constants.kInvalidID.Value;

      return aID;
    }

    public static HeaderId CopyNormalizeHexaID(HeaderId aID) =>
      NormalizeHexaID(new HeaderId(aID));

    public static HeaderId NormalizeRecurrenceForID(HeaderId aID, HeaderContent aHeaderContent)
    {
      while (IDValidator.IsRecurrentCase(aID, aHeaderContent))
        aID.Value = aHeaderContent.NameToID[aID.Value].Value;

      if (!IDValidator.IsValidWithoutRecurrenceCheck(aID))
        aID.Value = Constants.kInvalidID.Value;

      return aID;
    }

    public static HeaderId CopyNormalizeRecurrenceForID(HeaderId aID, HeaderContent aHeaderContent) =>
      NormalizeRecurrenceForID(new HeaderId(aID), aHeaderContent);
  }
}
