using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    // A) Monitor 클래스의 Enter와 Exit를 사용해 구문끼리 묶어서 잠굴 수 있다
    // B) Enter와 Exit 쌍을 맞춰주지 않으면 끝없이 기다리면서 멈춘다(데드락)
    // C) lock 키워드를 통해 조금이나마 편리하게 관리 가능함(자동으로 잠금, 해제)
    // D) obj == key
    class Program
    {
        private static int number = 0;
        private static object _obj = new object();
        static void Thread_1()
        {
            for (int i = 0; i < 100000; i++)
            {
                lock (_obj)
                {
                    number++;
                }
                
                // 상호배제 Mutual Exclusive
                // Monitor.Enter(_obj);    // 문을 잠구는 행위
                // number++;
                // Monitor.Exit(_obj);     // 잠금을 풀어준다
            }
        }
        
        static void Thread_2()
        {
            for (int i = 0; i < 100000; i++)
            {
                lock (_obj)
                {
                    number--;
                }
                
                // Monitor.Enter(_obj);
                // number--;
                // Monitor.Exit(_obj);
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