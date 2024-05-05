using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    // A) 같은 자원을 두개 이상의 스레드가 동시에 접근하면 여러 문제가 발생할 수 있음
    // B) 문제들을 막기 위해서는 연산에 원자성과 순서를 보장해 주어야 함
    // C) Interlocked 키워드를 통해 정수형 연산은 위 규칙을 지킬 수 있음
    // D) 중간 결과 값을 보고 싶을 때는 새로운 라인에서 변수를 출력하는게 아닌
    //    반환값을 받아서 출력해야함 (한줄 사이에 값이 바뀔 수 있음)
    class Program
    {
        private static int number = 0;
        static void Thread_1()
        {
            for (int i = 0; i < 100000; i++)
            {
                // All or Nothing
                int afterValue = Interlocked.Increment(ref number);
            }
        }
        
        static void Thread_2()
        {
            for (int i = 0; i < 100000; i++)
            {
                // All or Nothing
                int afterValue = Interlocked.Decrement(ref number);
            }
        }
        
        private static void Main(string[] args)
        {
            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);
            t1.Start();
            t2.Start();
            
            Task.WaitAll(t1, t2);
            
            Console.WriteLine(number);
        }
    }
}