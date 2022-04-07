using Akka.Actor;
using Akka.Configuration;

namespace producer
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
                            port = 8082
                            hostname = 0.0.0.0
                            public-hostname = localhost
                        }
                    }
                }
                ");

            using var system = ActorSystem.Create("producer-system", config);
                
            var producerActor = system.ActorOf<ProducerActor>("ProducerActor");

            while (true)
            {
                producerActor.Tell(Console.ReadLine());
            }
        }
    }


    public class ProducerActor : ReceiveActor
    {
        public ProducerActor()
        {
            Receive<string>(message =>
            {
                var selectionConsumer = Context.ActorSelection("akka.tcp://consumer-system@localhost:8081/user/ConsumerActor");

                selectionConsumer.Tell(message);
            });
        }
    }
}
