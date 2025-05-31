using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ConsumidorProducao.Models;

namespace ConsumidorProducao
{
    class Program
    {
        static int totalProcessadas = 0;
        static int totalFalhas = 0;
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "producao_exchange", type: "fanout");

                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queue: queueName, exchange: "producao_exchange", routingKey: "");

                Console.WriteLine("A aguardar mensagens de produção...\n");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var mensagem = Encoding.UTF8.GetString(body);
                    Console.WriteLine("Recebido: " + mensagem);

                    try
                    {
                        var dados = JsonConvert.DeserializeObject<MensagemProducao>(mensagem);

                        var produto = new Produto
                        {
                            Codigo_Peca = dados.codigo,
                            Data_Producao = DateTime.Parse(dados.data),
                            Hora_Producao = DateTime.Parse(dados.data).TimeOfDay,
                            Tempo_Producao = dados.tempo,
                            Testes = new List<Teste>
                        {
                            new Teste
                            {
                                Codigo_Resultado = dados.resultado,
                                Data_Teste = DateTime.Now
                            }
                        }
                        };

                        var json = JsonConvert.SerializeObject(produto);
                        var client = new WebClient();
                        client.Headers[HttpRequestHeader.ContentType] = "application/json";

                        string apiUrl = "https://localhost:7097/api/Produto";
                        client.UploadString(apiUrl, "POST", json);

                        Console.WriteLine("Enviado para a API com sucesso.");

                        totalProcessadas++;
                        if (dados.resultado != "01") totalFalhas++;

                        Console.WriteLine($"Total processadas: {totalProcessadas} | Total com falha: {totalFalhas}\n");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Erro ao enviar para API: " + ex.Message);
                    }
                };

                channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

                Console.WriteLine("Pressiona ENTER para sair.");
                Console.ReadLine();
            }
        }

        class MensagemProducao
        {
            public string codigo { get; set; }
            public string data { get; set; }
            public int tempo { get; set; }
            public string resultado { get; set; }
        }
    }
}