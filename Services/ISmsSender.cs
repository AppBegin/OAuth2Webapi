using System.Threading.Tasks;

namespace auth
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
