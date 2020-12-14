namespace Parsers
{
    public interface IParser
    {
        T Parse<T>(string filePath);
    }
}
