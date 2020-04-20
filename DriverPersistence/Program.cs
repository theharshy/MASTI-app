using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DriverPersistence
{
    class Program
    {
        static void Main(string[] args)
        {
            Persistence.Persistence p = new Persistence.Persistence();
            int result = p.DeleteSession(3, 4);
            Console.WriteLine(result);


            Thread.Sleep(10000);
        }
    }
}
