using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;

namespace PublicadorProducao
{
    class Program
    {
        static readonly string[] resultados = { "01", "02", "03", "04", "05", "06" };
        static readonly Random random = new Random();
        static void Main(string[] args)
        {
            bool sair = false;

            while (!sair)
            {
                Console.Clear();
                Console.WriteLine("===== Menu do Publisher =====");
                Console.WriteLine("1. Gerar 1 peça");
                Console.WriteLine("2. Gerar 2 peças");
                Console.WriteLine("3. Gerar 5 peças");
                Console.WriteLine("4. Gerar peças continuamente (CTRL+C para parar)");
                Console.WriteLine("0. Sair");
                Console.Write("Escolha: ");

                string opcao = Console.ReadLine();
                int quantidade = 0;

                switch (opcao)
                {
                    case "1":
                        quantidade = 1;
                        break;
                    case "2":
                        quantidade = 2;
                        break;
                    case "3":
                        quantidade = 5;
                        break;
                    case "4":
                        Console.WriteLine("\nA gerar peças continuamente (CTRL+C para parar)...\n");
                        while (true)
                        {
                            GerarEPublicarPeca();
                            Thread.Sleep(2000);
                        }
                    case "0":
                        sair = true;
                        continue;
                    default:
                        Console.WriteLine("Opção inválida. Prima ENTER para continuar...");
                        Console.ReadLine();
                        continue;
                }

                for (int i = 0; i < quantidade; i++)
                {
                    GerarEPublicarPeca();
                    Thread.Sleep(1000);
                }

                Console.WriteLine("\nPressiona ENTER para voltar ao menu...");
                Console.ReadLine();
            }
        }

        static void GerarEPublicarPeca()
        {
            string codigoPeca = GerarCodigoPeca();
            DateTime dataProducao = DateTime.Now;
            int tempoProducao = random.Next(10, 100);
            string resultado = resultados[random.Next(resultados.Length)];

            var mensagem = $"{{ \"codigo\": \"{codigoPeca}\", \"data\": \"{dataProducao:yyyy-MM-ddTHH:mm:ss}\", \"tempo\": {tempoProducao}, \"resultado\": \"{resultado}\" }}";

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "producao_exchange", type: "fanout");

                var body = Encoding.UTF8.GetBytes(mensagem);

                channel.BasicPublish(exchange: "producao_exchange",
                                     routingKey: "",
                                     basicProperties: null,
                                     body: body);
            }

            if (resultado == "01")
            {
                Console.WriteLine($"OK    - Peça: {codigoPeca}, Resultado: {resultado}, enviada para RabbitMQ");
            }
            else
            {
                Console.WriteLine($"FALHA - Peça: {codigoPeca}, Resultado: {resultado}, enviada para RabbitMQ");
            }
        }

        static string GerarCodigoPeca()
        {
            string[] prefixos = { "aa", "ab", "ba", "bb" };
            string prefixo = prefixos[random.Next(prefixos.Length)];
            int sufixo = random.Next(100000, 999999);
            return prefixo + sufixo.ToString();
        }
    }
}