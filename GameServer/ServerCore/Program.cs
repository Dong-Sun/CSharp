using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    // 함수 내에서 라인끼리 의존성이 없을 경우 하드웨어가 최적화를 위해 순서를 뒤바꿔서 실행을 함
    // 싱글 스레드에서는 문제 없지만 멀티 스레드 환경에서는 문제가 생길 우려가 있음
    // 그렇기에 경계선을 그어 순서가 섞이지 않게끔 관리해줘야 한다
    
    // 메모리 베리어
    // A) 코드 재배치 억제
    // B) 가시성
    
    // 1) Full Memory Barrier (ASM MFENCE, C# Thread.MemoryBarrier) Store/Load 둘다 막는다
    // 2) Store Memory Barrier (ASM SFENCE) : Store만 막는다
    // 3) Load Memory Barrier (ASM LFENCE) : Load만 막는다
    class Program
    {
        private static int x = 0;
        private static int y = 0;
        private static int r1 = 0;
        private static int r2 = 0;
        
        static void Thread_1()
        {
            y = 1; // Store y
            
            // ---------------
            Thread.MemoryBarrier();

            r1 = x; // Load x
        }
        
        static void Thread_2()
        {
            x = 1; // Store x
            
            // ---------------
            Thread.MemoryBarrier();

            r2 = y; // Load y
        }
        
        private static void Main(string[] args)
        {
            int count = 0;
            while (true)
            {
                count++;
                
                Task t1 = new Task(Thread_1);
                Task t2 = new Task(Thread_2);
                t1.Start();
                t2.Start();

                Task.WaitAll(t1, t2);
                if (r1 == 0 && r2 == 0)
                    break;
            }
            Console.WriteLine(count);
        }
    }
}