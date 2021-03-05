namespace ZTR.M7Config.Business.Parsers.ProtoParser.Parser
{
    public interface IProtoFileCompiler
    {
        void GenerateDllFromFile(string dllPath, string fileContent, string fileName);
    }
}
