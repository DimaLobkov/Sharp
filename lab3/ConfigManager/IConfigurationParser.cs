namespace ConfigManager
{
    public interface IConfigurationParser<out T>
    {
        T Parse();
    }
}