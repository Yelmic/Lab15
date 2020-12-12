using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Reflection;

namespace lab15
{
    class Loader
    {
        private object mutex = new object();
        private bool canClear = false;

        private List<int> objects = new List<int>();

        public int ThreadSleepTime { get; set; } = 0;
        public void Add(int obj)
        {
            canClear = false;
            lock (mutex)
            {
                objects.Add(obj);//записываем элементы потока в список
            }
            if (ThreadSleepTime > 0)
                Thread.Sleep(ThreadSleepTime);//приостанавливает поток на заданое кол-во сек
        }

        public void Clear()
        {
            if (canClear == true)
            {
                lock (mutex)
                {
                    objects.Clear();//очищаем список
                }
            }
        }


        public void LoadToFile(string path)
        {
            lock (mutex)
            {
                StreamWriter file = new StreamWriter(path);
                foreach (int obj in objects)
                {
                    file.WriteLine(obj);
                }
                file.Close();
                canClear = true;
            }
        }
    }
    class SortThread
    {
        private static Loader load = new Loader();
        public static void Method()
        {
            int n = 3;
            int[] objects = new int[n + 1];
            for (int i = 0; i <= n; i++)
            {
                Console.WriteLine(i);
                load.Add(i);
            }
        }
        public static void Dev1()
        {
            int n = 10;
            int[] objects = new int[n + 1];
            for (int i = 0; i <= n; i++)
            {
                if (i % 2 != 0)
                {
                    Console.WriteLine(i);
                    load.Add(i);
                }
            }
        }
        public static void Dev2()
        {
            int n = 10;
            int[] objects = new int[n + 1];
            for (int i = 0; i <= n; i++)
            {
                if (i % 2 == 0)
                {
                    Console.WriteLine(i);
                    load.Add(i);
                }
            }
        }

        public static void LoadToFile(string path)
        {
            load.LoadToFile(path);
            load.Clear();
        }

        public static void SetSleepTime(int time)//передаем сколько времени будет задержка
        {
            load.ThreadSleepTime = time;
        }
    }
    class Program
    {
        public static void Count(object n)
        {
            int x = (int)n;
            for (int i = 0; i < 6; i++, x++)
            {
                Console.WriteLine($"{x * i }");
            }
        }

        private static void TestSort()
        {
            SortThread.SetSleepTime(50);
            Thread one = new Thread(SortThread.Dev1);
            Thread two = new Thread(SortThread.Dev2);
            one.Priority = ThreadPriority.Lowest;
            two.Priority = ThreadPriority.Lowest;
            object mutex = new object();//синхронизирует поток
            one.Start();
            two.Start();
            while (two.IsAlive || one.IsAlive) ;
            SortThread.LoadToFile("DevSort.txt");
            SortThread.SetSleepTime(0);
        }


        static void Main(string[] args)
        {
             var AllProcess = Process.GetProcesses();
            foreach (Process p in AllProcess)
            {
                    Console.WriteLine(p.Id);
                    Console.WriteLine(p.ProcessName);
                    Console.WriteLine(p.BasePriority);
                    Console.WriteLine(p.HandleCount);
                    Console.WriteLine(p.MachineName);
                    Console.WriteLine();
            }



            AppDomain app = AppDomain.CurrentDomain;
            Console.WriteLine(app.FriendlyName);
            Console.WriteLine(app.SetupInformation);
            Assembly[] ass = app.GetAssemblies();
            foreach(Assembly s in ass)
            {
                Console.WriteLine(s.FullName);
            }
            AppDomain newapp = AppDomain.CreateDomain("newdom");
            newapp.Load("lab15");
            Console.WriteLine(newapp.FriendlyName);
            AppDomain.Unload(newapp);



            Console.WriteLine("new task");
            Thread newthread = new Thread(SortThread.Method);
            newthread.Start();
            newthread.Name = "mythread";
            Console.WriteLine(newthread.IsAlive);
            Console.WriteLine(newthread.Name);
            Console.WriteLine(newthread.Priority);
            Console.WriteLine(newthread.ManagedThreadId);
            while (newthread.IsAlive) ;
            SortThread.LoadToFile("method.txt");



            Console.WriteLine();
            Thread one = new Thread(SortThread.Dev1);
            Thread two = new Thread(SortThread.Dev2);
            one.Priority = ThreadPriority.BelowNormal;
            two.Start();
            one.Start();
            Thread.Sleep(1000);
            SortThread.LoadToFile("dev.txt");
            Console.WriteLine();
            TestSort();
            Console.ReadLine();



            
            int num = 1;
            TimerCallback timer = new TimerCallback(Count);
            Timer tm = new Timer(timer, num, 0,  2000);
            Console.ReadLine();
        }
    }
}
