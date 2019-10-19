using System.Threading.Tasks;

namespace WebTty.Common
{
    public interface IMessageDispatcher
    {
        Task<object> Dispatch(object message);
    }
}
