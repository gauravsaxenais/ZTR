namespace ZTR.M7Config.Business.Parsers.Core.Converter
{
    public interface IConverter<T>
    {
        T ToConverted(string json);

        string ToJson(string json);
    }
}
