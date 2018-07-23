using System.IO;

namespace StringEnhancer
{
  public abstract class Parse
  {
    public static readonly char[] kSplitResourceElementsChars = { ' ', '\t', ',', '.', ':', ';', '\\', '/', '!', '|',
      '?', '(', ')', ']', '[', '{', '}', '=', '+', '@', '\'', '"', '<', '>', '$', '%', '^', '&', '*', '~', '`', '-' };
    protected bool FileExists(string aPath) => File.Exists(aPath);
  }
}
