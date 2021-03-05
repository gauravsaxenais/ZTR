namespace ZTR.M7Config.Business.Parsers.Core.Converter
{
    public interface IExtractor<T>
    {
        public T Convert(object[] input);
    }
}
