using System.Security.Cryptography;

namespace ServerCore
{
    // TLS -> Thread Local Storage -> 각 스레드마다 갖고있는 전역 공간
    // 특정 부분에 작업량 늘어나게 되면 이를 집중처리 하려고 함
    // 이때 로직에 lock이 걸려있다면 한번에 하나씩 처리할 수 밖에 없음(lock을 걸고 해제하는 과정에서 싱글스레드보다 안좋아질 수도 있음)
    // 일감을 한번에 가져와서 작업하는게 해결하는게 효율적일 수도 있다(lock하는 과정이 줄어듬)
    class Program
    {
        private static ThreadLocal<string> ThreadName = new ThreadLocal<string>(() => { return $"My Name Is {Thread.CurrentThread.ManagedThreadId}"; });
        
        static void WhoAmI()
        {
            bool repeat = ThreadName.IsValueCreated;
            if (repeat)
                Console.WriteLine(ThreadName.Value + "(repeat)");
            else
                Console.WriteLine(ThreadName.Value);
        }
        private static void Main(string[] args)
        {
            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(3, 3);
            Parallel.Invoke(WhoAmI, WhoAmI, WhoAmI, WhoAmI,WhoAmI,WhoAmI,WhoAmI,WhoAmI);
            
            ThreadName.Dispose();
        }
    }
}
