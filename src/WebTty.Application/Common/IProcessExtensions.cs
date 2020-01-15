using System.Threading.Tasks;
using WebTty.Exec;

namespace WebTty.Application.Common
{
    public static class IProcessExtensions
    {
        public static async ValueTask WaitUntilReady(this IProcess proc)
        {
            if (!proc.IsRunning)
            {
                while (!proc.IsRunning)
                {
                    await Task.Delay(1);
                }
            }
        }
    }
}
