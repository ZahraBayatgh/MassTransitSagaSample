using System;

namespace Processor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting orden processor");

            var service = new ProcessorService();
            service.Start();

            Console.WriteLine("Press any key to close...");
            Console.ReadKey();

            service.Stop();
        }
    }
}
