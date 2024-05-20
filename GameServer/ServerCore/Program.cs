namespace ServerCore
{
	// AutoResetEvent -> 자동문
	// A) bool으로 입장 가능여부를 확인
	// B) 입장과 동시에 자동으로 문을 닫기에 원자성을 보장, lcok기능을 대체하기 적합하다
	// ManualResetEvent -> 수동문
	// A) 입장과 문을 닫는 행위가 분리되어 있어 lock을 대체하기엔 적합하지 않음
	// B) 하지만 수동으로 여닫는 특성은 여러 스레드를 한번에 통과시키거나 제한시키는 등에 사용 가능
	// Mutex
	// A) AutoResetEvent와 비슷하지만 int를 사용해 몇번을 풀어야 해제될지 정할 수 있음
	// B) 그리고 Thread ID가 있어서 lock을 해제한 스레드와 잠근 스레드가 다른 애인지 체크 가능
	// C) 더 많은 기능이 있기에 AutoResetEvent보다 비용이 더 크다
	class Lock
	{
		// bool <- 커널
		private ManualResetEvent _available = new ManualResetEvent(true);
		public void Acquire()
		{
			_available.WaitOne(); // 입장 시도
			_available.Reset(); // 문을 닫는다
		}
		public void Release()
		{
			_available.Set(); // flag = true
		}
	}
    class Program
    {
		static int _num = 0;
		static Mutex _lock = new Mutex();

		static void Thread_1()
		{
			for (int i = 0; i < 100000; i++)
			{
				_lock.WaitOne();
				_num++;
				_lock.ReleaseMutex();
			}
		}
		
		static void Thread_2()
		{
			for (int i = 0; i < 100000; i++)
			{
				_lock.WaitOne();
				_num--;
				_lock.ReleaseMutex();
			}
		}
		
        private static void Main(string[] args)
        {
			Task t1 = new Task(Thread_1);
			Task t2 = new Task(Thread_2);
			t1.Start();
			t2.Start();
			
			Task.WaitAll(t1, t2);
			
			System.Console.WriteLine(_num);
        }
    }
}
