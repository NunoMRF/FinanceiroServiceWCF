using System;
using System.Text;
using RabbitMQ.Client;

namespace PublicadorProducao
{
    class Program
    {
        static void Main(string[] args)
        {
            // Dados de produção simulados
            string codigoPeca = "bb569690";
            DateTime dataProducao = DateTime.Now;
            int tempoProducao = 45;
            string resultadoTeste = "F"; // "F" = falha; "P" = passou

            // Conectar ao RabbitMQ
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using (IConnection connection = factory.CreateConnection())
            using (IModel channel = connection.CreateModel())
            {
                // Declara uma exchange (tipo fanout para broadcast)
                channel.ExchangeDeclare(exchange: "producao_exchange", type: "fanout");

                // Criar a mensagem no formato JSON simples
                string mensagem = $"{{ \"codigo\": \"{codigoPeca}\", \"data\": \"{dataProducao:yyyy-MM-ddTHH:mm:ss}\", \"tempo\": {tempoProducao}, \"resultado\": \"{resultadoTeste}\" }}";

                var body = Encoding.UTF8.GetBytes(mensagem);

                // Publicar a mensagem
                channel.BasicPublish(exchange: "producao_exchange",
                                     routingKey: "",
                                     basicProperties: null,
                                     body: body);

                Console.WriteLine("✔ Mensagem enviada com sucesso:");
                Console.WriteLine(mensagem);
            }

            Console.WriteLine("Prima qualquer tecla para sair...");
            Console.ReadKey();
        }
    }
}
