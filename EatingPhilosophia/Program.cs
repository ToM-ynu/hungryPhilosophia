using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EatingPhilosophia
{
    class Program
    {
        const int philo_num = 5;
        const int waitingTime = 1000;
        static bool[] chopstic = { true, true, true, true, true };
        static void Main(string[] args)
        {
            Parallel.For(0, philo_num, i =>
            {
                PhilosophiaEating_undead2(i);
            });
            Console.ReadLine();
        }
        private static object syncObject = new object();

        static void PhilosophiaEating_dead(int i)//デットロックが発生する
        {
            int tempi = i;
            Philosophia philosophia = new Philosophia();
            //getchopstic
            for (int j = 0; j < 1000; j++)
            {
                while (philosophia.checkEating() == false)
                {
                    while (chopstic[i] == false)
                    {
                        Console.WriteLine("Philosophia:{0}: is waiting while Rightchopstic became available", tempi);
                        System.Threading.Thread.Sleep(waitingTime);
                    }
                    lock (syncObject)
                    {
                        //critical section
                        chopstic[i] = false;
                        //critical section
                        Console.WriteLine("Philosophia:{0}: is get Rightchopstic", tempi);
                        System.Threading.Thread.Sleep(waitingTime);
                    }
                    philosophia.getRightchopstic();
                    i = i + 1;//i+1を左側の手。１番大きい番号のi左手は0
                    if (philo_num <= i)
                    {
                        i = 0;
                    }
                    while (chopstic[i] == false)
                    {
                        Console.WriteLine("Philosophia:{0}: is waiting while Leftchopstic became available", tempi);
                        System.Threading.Thread.Sleep(waitingTime);
                    }
                    lock (syncObject)
                    {
                        //critical section
                        chopstic[i] = false;
                        //critical section
                        Console.WriteLine("Philosophia:{0}: is get Leftchopstic", tempi);
                    }
                }
                if (philosophia.checkEating())
                {
                    Console.WriteLine("Philosophia:{0}: is Eating", tempi);
                }
                philosophia.releaseLeftchopstic();
                philosophia.releaseRightchopstic();
                lock (syncObject)
                {
                    //critical section
                    chopstic[i] = true;
                    chopstic[tempi] = true;
                    //critical section
                }
                Console.WriteLine("Philosophia:{0}: release both chopstic", tempi);
                //release chopstic
                philosophia.finishEating();
                System.Threading.Thread.Sleep(waitingTime / 5);
            }
        }
        static void PhilosophiaEating_undead(int i)/*半順序によるdead lock の回避*/
        {

            int right = i;
            int left = i + 1;
            if (i == philo_num - 1)
            {
                left = 0;
            }
            int max = (int)Math.Max(right, left);
            int min = (int)Math.Min(left, right);
            //哲学者は必ず数字の大きい方から箸を取り出す
            Philosophia philosophia = new Philosophia();
            //getchopstic
            for (int j = 0; j < 5; j++)
            {
                while (philosophia.checkEating() == false)
                {
                    while (chopstic[max] == false)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("Philosophia:{0}: is waiting while :{1}:chopstic became available A", i, max);
                        Console.ResetColor();
                        System.Threading.Thread.Sleep(waitingTime);
                    }
                    lock (syncObject)
                    {
                        //critical section
                        chopstic[max] = false;
                        //critical section
                        Console.WriteLine("Philosophia:{0}: is get :{1}:chopstic B", i, max);
                        System.Threading.Thread.Sleep(waitingTime);
                    }
                    philosophia.getRightchopstic();
                    while (chopstic[min] == false)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("Philosophia:{0}: is waiting while :{1}:chopstic became available C", i, min);
                        System.Threading.Thread.Sleep(waitingTime);
                        Console.ResetColor();
                    }
                    lock (syncObject)
                    {
                        //critical section
                        chopstic[min] = false;
                        //critical section
                        Console.WriteLine("Philosophia:{0}: is get :{1}:chopstic D", i, min);
                    }
                    philosophia.getLeftchopstic();
                }
                if (philosophia.checkEating())
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Philosophia:{0}: is Eating", i);
                    Console.ResetColor();
                }
                philosophia.releaseLeftchopstic();
                philosophia.releaseRightchopstic();
                lock (syncObject)
                {
                    //critical section
                    //大きい方から置く
                    chopstic[max] = true;
                    chopstic[min] = true;
                    //critical section
                }
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Philosophia:{0}: release both chopstic", i);
                Console.ResetColor();
                philosophia.releaseLeftchopstic();
                philosophia.releaseRightchopstic();
                philosophia.finishEating();
                //release chopstic
                System.Threading.Thread.Sleep(waitingTime / 5);
            }
        }
        static void PhilosophiaEating_undead2(int i)/*両の手で箸が取れなかったとき、箸を元の位置に戻す*/
        {
            int right = i;
            int left = i + 1;
            if (i == philo_num - 1)
            {
                left = 0;
            }
            Philosophia philosophia = new Philosophia();
            //getchopstic
            for (int j = 0; j < 10; j++)
            {
                while (philosophia.checkEating() == false)
                {
                label1:
                    while (chopstic[right] == false)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("Philosophia:{0}: is waiting while Rightchopstic became available", i);
                        Console.ResetColor();
                        System.Threading.Thread.Sleep(waitingTime);
                    }
                    lock (syncObject)
                    {
                        //critical section
                        chopstic[right] = false;
                        //critical section
                        Console.WriteLine("Philosophia:{0}: is get Rightchopstic", i);
                        System.Threading.Thread.Sleep(waitingTime);
                    }
                    philosophia.getRightchopstic();
                    while (chopstic[left] == false)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("Philosophia:{0}: is waiting while Leftchopstic became available", i);
                        Console.ResetColor();
                        System.Threading.Thread.Sleep(waitingTime);
                        //変更点
                        philosophia.releaseRightchopstic();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Philosophia:{0}: release right chopstic", i);
                        Console.ResetColor();
                        lock (syncObject)
                        {
                            //critical section
                            chopstic[right] = true;
                            //critical section
                            System.Threading.Thread.Sleep(waitingTime);
                        }
                        philosophia.releaseRightchopstic();
                        goto label1;
                    }
                    lock (syncObject)
                    {
                        //critical section
                        chopstic[left] = false;
                        //critical section
                        Console.WriteLine("Philosophia:{0}: is get Leftchopstic", i);
                    }
                    philosophia.getLeftchopstic();
                }
                if (philosophia.checkEating())
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Philosophia:{0}: is Eating", i);
                    Console.ResetColor();
                }
                philosophia.releaseLeftchopstic();
                philosophia.releaseRightchopstic();
                lock (syncObject)
                {
                    //critical section
                    chopstic[right] = true;
                    chopstic[left] = true;
                    //critical section
                }
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Philosophia:{0}: release both chopstic", i);
                Console.ResetColor();
                //release chopstic
                philosophia.finishEating();
                System.Threading.Thread.Sleep(waitingTime / 5);
            }

        }
        static void PhilosophiaEating_undead3(int i)/*ウェイターを配置する*/
        {
            int right = i;
            int left = i + 1;
            if (i == philo_num - 1)
            {
                left = 0;
            }
            Philosophia philosophia = new Philosophia();
            //getchopstic
            for (int j = 0; j < 10; j++)
            {
                while (philosophia.checkEating() == false)
                {
                label1:
                    while (chopstic[right] == false)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("Philosophia:{0}: is waiting while Rightchopstic became available", i);
                        Console.ResetColor();
                        System.Threading.Thread.Sleep(waitingTime);
                    }
                    lock (syncObject)
                    {
                        //critical section
                        chopstic[right] = false;
                        //critical section
                        Console.WriteLine("Philosophia:{0}: is get Rightchopstic", i);
                        System.Threading.Thread.Sleep(waitingTime);
                    }
                    philosophia.getRightchopstic();
                    while (chopstic[left] == false)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("Philosophia:{0}: is waiting while Leftchopstic became available", i);
                        Console.ResetColor();
                        System.Threading.Thread.Sleep(waitingTime);
                        //変更点
                        philosophia.releaseRightchopstic();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Philosophia:{0}: release right chopstic", i);
                        Console.ResetColor();
                        lock (syncObject)
                        {
                            //critical section
                            chopstic[right] = true;
                            //critical section
                            System.Threading.Thread.Sleep(waitingTime);
                        }
                        philosophia.releaseRightchopstic();
                        goto label1;
                    }
                    lock (syncObject)
                    {
                        //critical section
                        chopstic[left] = false;
                        //critical section
                        Console.WriteLine("Philosophia:{0}: is get Leftchopstic", i);
                    }
                    philosophia.getLeftchopstic();
                }
                if (philosophia.checkEating())
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Philosophia:{0}: is Eating", i);
                    Console.ResetColor();
                }
                philosophia.releaseLeftchopstic();
                philosophia.releaseRightchopstic();
                lock (syncObject)
                {
                    //critical section
                    chopstic[right] = true;
                    chopstic[left] = true;
                    //critical section
                }
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Philosophia:{0}: release both chopstic", i);
                Console.ResetColor();
                //release chopstic
                philosophia.finishEating();
                System.Threading.Thread.Sleep(waitingTime / 5);
            }

        }
        public class Philosophia
        {
            private bool lefthand = false;
            private bool righthand = false;
            private bool eating = false;
            public void getLeftchopstic()
            {
                lefthand = true;
                //checkEating();
            }
            public void getRightchopstic()
            {
                righthand = true;
                //checkEating();
            }
            public void releaseLeftchopstic()
            {
                lefthand = false;
                checkEating();
            }
            public void releaseRightchopstic()
            {
                righthand = false;
                //checkEating();
            }
            public bool checkEating()
            {
                if (lefthand == true && righthand == true)
                {
                    eating = true;
                }
                return eating;
            }
            public void finishEating()
            {
                eating = false;
            }
        }
    }
}