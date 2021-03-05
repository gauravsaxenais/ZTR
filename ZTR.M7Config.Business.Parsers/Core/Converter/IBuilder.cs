namespace ZTR.M7Config.Business.Parsers.Core.Converter
{
    using Models;
    public interface IBuilder<T>
    {
        string ToTOML(T content, ValueScheme scheme);
        T ToDictionary(string toml);
    }
}
