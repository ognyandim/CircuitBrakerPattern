namespace IntegrationTests
{
    public interface IDummyService
    {
        string Get(string paremeterKey);
        void Put(string paremeterKey, string parameterValue);
    }
}