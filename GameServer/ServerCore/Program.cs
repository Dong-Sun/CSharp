namespace ServerCore
{
	// 컨텍스트 스위칭(Context Switching) => CPU/코어에서 실행할 프로세스 or 스레드를 교체하는 기술
	// A) 코어가 다른 스레드로 이동할 때 기존 정보를 저장하고 새로운 정보를 복원하는 과정이 필요
	// B) 이 과정이 부담이 될수 있어서 스레드를 교체하는게 Spin Lcok보다 무조건 좋다고 보장할 수는 없다.
	class SpinLock
	{
		volatile int _locked = 0;

		public void Acquire()
		{
			while (true)
			{
				// int original = Interlocked.Exchange(ref _locked, 1);
				// if (original == 0)
				//	break;

				// CAS Compare-And-Swap
				int expected = 0;
				int desired = 1;
				if (Interlocked.CompareExchange(ref _locked, desired, expected) == expected)
					break;
				// Thread.Sleep(1); // 무조건 휴식 => 무조건 1ms 정도 쉰다.
				// Thread.Sleep(0); // 조건부 양보 => 나보다 우선수위가 낮은 애들한테는 양보 불가 => 우선순위가 나보다 같거나 높은 쓰레드가 없으면 다시 본인한테
				Thread.Yield(); // 관대한 양보 => 지금 실행이 가능한 쓰레드에 양보 => 실행 가능한 애가 없으면 남은 시간 소진
			}
		}
		public void Release()
		{
			_locked = 0;
		}
	}
    class Program
    {
		static int _num = 0;
		static SpinLock _lock = new SpinLock();

		static void Thread_1()
		{
			for (int i = 0; i < 100000; i++)
			{
				_lock.Acquire();
				_num++;
				_lock.Release();
			}
		}
		
		static void Thread_2()
		{
			for (int i = 0; i < 100000; i++)
			{
				_lock.Acquire();
				_num--;
				_lock.Release();
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
