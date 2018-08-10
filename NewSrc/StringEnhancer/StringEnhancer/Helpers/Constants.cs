namespace StringEnhancer
{
  public static class Constants
  {
    public const int kStringTableCapacity = 16;
    public static HeaderId kInvalidID = new HeaderId("___INVALID_ID");
    public const int kDuplicateID = -63;
    public const int kMaxID = 65535;
    public static HeaderId kNotFoundID = new HeaderId("NOT_FOUND");
    public const int kNotDiscovered = -1;
    public static readonly char[] kSplitTokens = { ' ', '\t' };
    public const int kMinID = 0;
  }
}
