using System.Collections;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace HomaGames.GameDoctor.Tests
{
    public static class TestUtils
    {
        public static IEnumerator AsIEnumerator(Task task)
        {
            while (!task.IsCompleted)
            {
                yield return null;
            }
 
            if (task.IsFaulted)
            {
                ExceptionDispatchInfo.Capture(task.Exception).Throw();
            }
 
            yield return null;
        }
        public static IEnumerator AsIEnumerator<T>(Task<T> task)
        {
            while (!task.IsCompleted)
            {
                yield return null;
            }
 
            if (task.IsFaulted)
            {
                ExceptionDispatchInfo.Capture(task.Exception).Throw();
            }
 
            yield return null;
        }
    }
}