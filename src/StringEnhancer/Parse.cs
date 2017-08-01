using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caphyon.RcStrings.StringEnhancer
{
  public abstract class Parse
  {
    public static readonly char[] kSplitResourceElementsChars = { ' ', '\t', ',', '.', '\\', '/', '!', '?', '(', ')', ']', '[', '{', '}', '=' };
    protected bool FileExists(string aPath) => File.Exists(aPath);
  }
}
