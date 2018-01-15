using System;
using System.IO.Pipes;

namespace ConsoleServerApp
{
    class ConsoleServerApp
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Server");

            var pipe = new NamedPipeServerStream("MyTest.Pipe", PipeDirection.InOut);
            Console.WriteLine("Waiting for connection....");
            pipe.WaitForConnection();

            Console.WriteLine("Connected");
        }
    }
}
