using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ConsumidorProducao
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                // Garante que a mesma exchange usada pelo Publisher existe
                channel.ExchangeDeclare(exchange: "producao_exchange", type: "fanout");

                // Cria uma queue temporária para este consumidor
                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queue: queueName, exchange: "producao_exchange", routingKey: "");

                Console.WriteLine("A aguardar mensagens com falha...\n");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var mensagem = Encoding.UTF8.GetString(body);
                    Console.WriteLine("Recebido: " + mensagem);
                };

                channel.BasicConsume(queue: queueName,
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine("Pressiona ENTER para sair.");
                Console.ReadLine();
            }
        }
    }
}