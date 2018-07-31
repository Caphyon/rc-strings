namespace StringEnhancer
{
  public static class Constants
  {
    public const int kStringTableCapacity = 16;
    public static HeaderId kInvalidID = new HeaderId("___INVALID_ID");
    public const int kDuplicateID = -63;
    public const int kMaxID = 65535;
    public static HeaderId kNotFoundID = new HeaderId("NOT_EXISTING");
    public const int kNotDiscovered = -1;
  }
}
