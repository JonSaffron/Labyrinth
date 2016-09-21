using System;
using Labyrinth.Services.PathFinder;
using NUnit.Framework;

namespace Labyrinth.Test
    {
    [TestFixture]
    class PriorityQueueTest
        {
        [Test]
        public void TestPriorityQueue(string[] args)
            {
            Console.WriteLine("\nBegin Priority Queue demo");
      
            Console.WriteLine("\nCreating priority queue of Employee items\n");
            PriorityQueue<Employee> pq = new PriorityQueue<Employee>();

            Employee e1 = new Employee("Aiden", 1.0);
            Employee e2 = new Employee("Baker", 2.0);
            Employee e3 = new Employee("Chung", 3.0);
            Employee e4 = new Employee("Dunne", 4.0);
            Employee e5 = new Employee("Eason", 5.0);
            Employee e6 = new Employee("Flynn", 6.0);

            Console.WriteLine("Adding " + e5 + " to priority queue");
            pq.Enqueue(e5);
            Console.WriteLine("Adding " + e3 + " to priority queue");
            pq.Enqueue(e3);
            Console.WriteLine("Adding " + e6 + " to priority queue");
            pq.Enqueue(e6);
            Console.WriteLine("Adding " + e4 + " to priority queue");
            pq.Enqueue(e4);
            Console.WriteLine("Adding " + e1 + " to priority queue");
            pq.Enqueue(e1);
            Console.WriteLine("Adding " + e2 + " to priority queue");
            pq.Enqueue(e2);

            Console.WriteLine("\nPriory queue is: ");
            Console.WriteLine(pq.ToString());
            Console.WriteLine("\n");

            Console.WriteLine("Removing an employee from priority queue");
            Employee e = pq.Dequeue();
            Console.WriteLine("Removed employee is " + e);
            Console.WriteLine("\nPriory queue is now: ");
            Console.WriteLine(pq.ToString());
            Console.WriteLine("\n");
            
            Console.WriteLine("Removing a second employee from queue");
            pq.Dequeue();
            Console.WriteLine("\nPriory queue is now: ");
            Console.WriteLine(pq.ToString());
            Console.WriteLine("\n");

            Console.WriteLine("\nEnd Priority Queue demo");
            Console.ReadLine();
            }

        [Test]
        public void TestPriorityQueue()
            {
            TestPriorityQueue(50000);
            }

        private static void TestPriorityQueue(int numOperations)
            {
            Random rand = new Random(0);
            PriorityQueue<Employee> pq = new PriorityQueue<Employee>();
            for (int op = 0; op < numOperations; ++op)
                {
                int opType = rand.Next(0, 2);

                if (opType == 0) // enqueue
                    {
                    string lastName = op + "man";
                    double priority = (100.0 - 1.0) * rand.NextDouble() + 1.0;
                    pq.Enqueue(new Employee(lastName, priority));
                    if (pq.IsConsistent() == false)
                        {
                        throw new InvalidOperationException("Test fails after enqueue operation # " + op);
                        }
                    }
                else // dequeue
                    {
                    if (pq.Count() > 0)
                        {
                        pq.Dequeue();
                        if (pq.IsConsistent() == false)
                            {
                            throw new InvalidOperationException("Test fails after dequeue operation # " + op);
                            }
                        }
                    }
                }
            Console.WriteLine("\nAll tests passed");
            }

        public class Employee : IComparable<Employee>
            {
            public string LastName { get; set; }
            public double Priority { get; set; }

            public Employee(string lastName, double priority)
                {
                this.LastName = lastName;
                this.Priority = priority;
                }

            public override string ToString()
                {
                return "(" + LastName + ", " + Priority.ToString("F1") + ")";
                }

            public int CompareTo(Employee other)
                {
                if (this.Priority < other.Priority) 
                    return -1;
                if (this.Priority > other.Priority) 
                    return 1;
                return 0;
                }
            }
        }
    }
