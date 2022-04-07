
using Akka.Actor;
using Akka.Configuration;

namespace consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigurationFactory.ParseString(@"
                akka {  
                    actor {
                        provider = remote
                    }
                    remote {
                        dot-netty.tcp {
                            port = 8081
                            hostname = 0.0.0.0
                            public-hostname = localhost
                        }
                    }
                }
                ");

            using var system = ActorSystem.Create("consumer-system", config);

            var consumerActor = system.ActorOf<ConsumerActor>("ConsumerActor");

            Console.ReadLine();
        }

    }


    public class ConsumerActor : ReceiveActor
    {
        public ConsumerActor()
        {
            var x = Self.Path.ToString();
            Receive<string>(message => Console.WriteLine($" Received message: {message}", ConsoleColor.Green));
        }
    }
}