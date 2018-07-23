namespace StringEnhancer
{
  public interface IBuilder<TBuiltObject>
  {
    void Build();
    TBuiltObject GetResult();
  }
}
