using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caphyon.RcStrings.StringEnhancer
{
  public class GenericComparer<T> : IComparer<T>
  {
    private readonly Func<T, T, int> mPredicate;

    public GenericComparer(Func<T, T, int> aPredicate)
    {
      this.mPredicate = aPredicate;
    }

    public int Compare(T x, T y)
    {
      return mPredicate(x, y);
    }
  }

  public class GenericEqComparer<T> : IEqualityComparer<T>
  {
    private readonly Func<T, T, bool> mPredicate;

    public GenericEqComparer(Func<T, T, bool> aPredicate)
    {
      this.mPredicate = aPredicate;
    }

    public bool Equals(T x, T y)
    {
      return mPredicate(x, y);
    }

    public int GetHashCode(T obj)
    {
      return base.GetHashCode();
    }
  }

}
