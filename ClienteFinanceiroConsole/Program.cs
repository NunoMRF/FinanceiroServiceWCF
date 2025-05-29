using System;
using ClienteFinanceiroConsole.FinanceiroService; // Usa o namespace que escolheste na "Service Reference"

namespace ClienteFinanceiroConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new FinanceiroServiceClient();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== MENU FINANCEIRO ===");
                Console.WriteLine("1 - GetPecaMaiorPrejuizo");
                Console.WriteLine("2 - GetCustoTotalProducao");
                Console.WriteLine("3 - GetLucroTotal");
                Console.WriteLine("4 - GetPrejuizoTotalPorPeca");
                Console.WriteLine("5 - GetDadosFinanceirosPorPeca");
                Console.WriteLine("0 - Sair");
                Console.Write("Escolha a opção: ");
                string opcao = Console.ReadLine();

                switch (opcao)
                {
                    case "1":
                        var peca = client.GetPecaMaiorPrejuizo();
                        Console.WriteLine("Peça com maior prejuízo: " + peca);
                        break;

                    case "2":
                        var inicio2 = LerData("Data início (yyyy-MM-dd): ");
                        var fim2 = LerData("Data fim (yyyy-MM-dd): ");
                        var custo = client.GetCustoTotalProducao(inicio2, fim2);
                        Console.WriteLine("Custo total: " + custo);
                        break;

                    case "3":
                        var inicio3 = LerData("Data início (yyyy-MM-dd): ");
                        var fim3 = LerData("Data fim (yyyy-MM-dd): ");
                        var lucro = client.GetLucroTotal(inicio3, fim3);
                        Console.WriteLine("Lucro total: " + lucro);
                        break;

                    case "4":
                        var inicio4 = LerData("Data início (yyyy-MM-dd): ");
                        var fim4 = LerData("Data fim (yyyy-MM-dd): ");
                        var lista = client.GetPrejuizoTotalPorPeca(inicio4, fim4);
                        foreach (var item in lista)
                        {
                            Console.WriteLine($"Peça: {item.CodigoPeca} - Prejuízo: {item.TotalPrejuizo}");
                        }
                        break;

                    case "5":
                        Console.Write("Código da peça: ");
                        string codigo = Console.ReadLine();
                        var dados = client.GetDadosFinanceirosPorPeca(codigo.Trim());
                        if (dados != null)
                        {
                            Console.WriteLine($"Peça: {dados.CodigoPeca}");
                            Console.WriteLine($"Custo: {dados.Custo}");
                            Console.WriteLine($"Lucro: {dados.Lucro}");
                            Console.WriteLine($"Prejuízo: {dados.Prejuizo}");
                            Console.WriteLine($"Tempo Produção: {dados.TempoProducao}");
                        }
                        else
                        {
                            Console.WriteLine("Peça não encontrada.");
                        }
                        break;

                    case "0":
                        client.Close();
                        return;

                    default:
                        Console.WriteLine("Opção inválida!");
                        break;
                }

                Console.WriteLine("\nPressiona ENTER para continuar...");
                Console.ReadLine();
            }
        }

        static DateTime LerData(string mensagem)
        {
            Console.Write(mensagem);
            string entrada = Console.ReadLine();
            DateTime data;
            while (!DateTime.TryParse(entrada, out data))
            {
                Console.Write("Data inválida. Tenta novamente: ");
                entrada = Console.ReadLine();
            }
            return data;
        }
    }
}
