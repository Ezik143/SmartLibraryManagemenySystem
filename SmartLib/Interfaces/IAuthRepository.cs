namespace SmartLib.Interfaces
{
    public interface IAuthRepository
    {
       Task RegisterAsync(string username, string password);
       Task<string> LoginAsync(string username, string password);
    }
}
