using System;
using Labyrinth.Services.PathFinder;
using NUnit.Framework;

// ReSharper disable MustUseReturnValue

namespace Labyrinth.Test
    {
    [TestFixture]
    class PriorityQueueTest
        {
        [Test]
        public void TestPriorityQueue()
            {
            Console.WriteLine("\nBegin Priority Queue demo");
      
            Console.WriteLine("\nCreating priority queue of Employee items\n");
            var pq = new PriorityQueue<double, Employee>();

            Employee e1 = new Employee("Aiden", 1.0);
            Employee e2 = new Employee("Baker", 2.0);
            Employee e3 = new Employee("Chung", 3.0);
            Employee e4 = new Employee("Dunne", 4.0);
            Employee e5 = new Employee("Eason", 5.0);
            Employee e6 = new Employee("Flynn", 6.0);

            Console.WriteLine("Adding " + e5 + " to priority queue");
            pq.Enqueue(e5.Priority, e5);
            Console.WriteLine("Adding " + e3 + " to priority queue");
            pq.Enqueue(e3.Priority, e3);
            Console.WriteLine("Adding " + e6 + " to priority queue");
            pq.Enqueue(e6.Priority, e6);
            Console.WriteLine("Adding " + e4 + " to priority queue");
            pq.Enqueue(e4.Priority, e4);
            Console.WriteLine("Adding " + e1 + " to priority queue");
            pq.Enqueue(e1.Priority, e1);
            Console.WriteLine("Adding " + e2 + " to priority queue");
            pq.Enqueue(e2.Priority, e2);

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
            }

        [Test]
        public void TestCantDequeue()
            {
            Employee e1 = new Employee("Aiden", 1.0);
            Employee e2 = new Employee("Baker", 2.0);
            Employee e3 = new Employee("Chung", 3.0);
            Employee e4 = new Employee("Dunne", 4.0);
            Employee e5 = new Employee("Eason", 5.0);
            Employee e6 = new Employee("Flynn", 6.0);
            var pq = new PriorityQueue<double, Employee>();
            pq.Enqueue(e1.Priority, e1);
            pq.Enqueue(e2.Priority, e2);
            pq.Enqueue(e3.Priority, e3);
            pq.Enqueue(e4.Priority, e4);
            pq.Enqueue(e5.Priority, e5);
            pq.Enqueue(e6.Priority, e6);
            pq.Dequeue();
            pq.Dequeue();
            pq.Dequeue();
            pq.Dequeue();
            pq.Dequeue();
            pq.Dequeue();

            Assert.Throws<InvalidOperationException>(() => pq.Dequeue());
            }

        [Test]
        public void TestToString()
            {
            Employee e1 = new Employee("Aiden", 1.0);
            Employee e2 = new Employee("Baker", 2.0);
            Employee e3 = new Employee("Chung", 3.0);
            Employee e4 = new Employee("Dunne", 4.0);
            Employee e5 = new Employee("Eason", 5.0);
            Employee e6 = new Employee("Flynn", 6.0);
            var pq = new PriorityQueue<double, Employee>();
            Assert.IsTrue(pq.ToString() == "(empty)");

            pq.Enqueue(e1.Priority, e1);
            pq.Enqueue(e2.Priority, e2);
            pq.Enqueue(e3.Priority, e3);
            pq.Enqueue(e4.Priority, e4);
            pq.Enqueue(e5.Priority, e5);
            pq.Enqueue(e6.Priority, e6);
            Assert.IsTrue(pq.ToString().Length > 20);
            }


        [Test]
        public void TestPriorityQueueRandom()
            {
            TestPriorityQueue(50000);
            }

        private static void TestPriorityQueue(int numOperations)
            {
            Random rand = new Random(0);
            var pq = new PriorityQueue<double, Employee>(1000);
            for (int op = 0; op < numOperations; ++op)
                {
                int opType = rand.Next(0, 2);

                if (opType == 0) // enqueue
                    {
                    string lastName = op + "man";
                    double priority = (100.0 - 1.0) * rand.NextDouble() + 1.0;
                    pq.Enqueue(priority, new Employee(lastName, priority));
                    if (pq.IsConsistent() == false)
                        {
                        throw new InvalidOperationException("Test fails after enqueue operation # " + op);
                        }
                    }
                else // dequeue
                    {
                    if (pq.Count > 0)
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

        public class Employee
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
            }
        }
    }
