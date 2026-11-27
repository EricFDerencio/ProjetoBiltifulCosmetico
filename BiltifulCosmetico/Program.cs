
using System;
using System.Globalization;
using BiltifulCosmetico.Classes;
using BiltifulCosmetico.Utils;

namespace BiltifulCosmetico
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Prepara o arquivo para os testes
            Dados.ProductionFiles.FileValidation(Dados.ProductionFiles.caminho);

            int opcao;

            do
            {
                Console.Clear();

                Console.WriteLine("================================");
                Console.WriteLine("   TESTE DA CLASSE PRODUCTION");
                Console.WriteLine("================================");
                Console.WriteLine("1 - Cadastrar produção");
                Console.WriteLine("2 - Listar produções");
                Console.WriteLine("3 - Localizar produção");
                Console.WriteLine("4 - Excluir produção");
                Console.WriteLine("5 - Testar limite de quantidade");
                Console.WriteLine("6 - Testar limite de IDs");
                Console.WriteLine("0 - Sair");
                Console.WriteLine("================================");
                Console.Write("Escolha uma opção: ");

                if (!int.TryParse(Console.ReadLine(), out opcao))
                {
                    opcao = -1;
                }

                Console.Clear();

                switch (opcao)
                {
                    case 1:
                        Cadastrar();
                        break;

                    case 2:
                        Listar();
                        break;

                    case 3:
                        Localizar();
                        break;

                    case 4:
                        Excluir();
                        break;

                    case 5:
                        TestarQuantidade();
                        break;

                    case 6:
                        TestarId();
                        break;

                    case 0:
                        Console.WriteLine("Encerrando o programa...");
                        break;

                    default:
                        Console.WriteLine("Opção inválida!");
                        break;
                }

                if (opcao != 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("Pressione ENTER para continuar...");
                    Console.ReadLine();
                }

            } while (opcao != 0);
        }

        static void Cadastrar()
        {
            try
            {
                Console.WriteLine("=== CADASTRAR PRODUÇÃO ===");

                Console.Write("Código do produto: ");
                string produto = Console.ReadLine();

                Console.Write("Quantidade produzida: ");
                float quantidade = float.Parse(
                    Console.ReadLine(),
                    CultureInfo.InvariantCulture
                );

                Production nova = new Production(produto.ToString(), quantidade);

                nova.Register(nova);

                Console.WriteLine();
                Console.WriteLine("Produção cadastrada com sucesso!");
                Console.WriteLine(nova);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine("Erro de quantidade: " + e.Message);
            }
            catch (FormatException)
            {
                Console.WriteLine("Digite valores numéricos válidos.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Erro: " + e.Message);
            }
        }

        static void Listar()
        {
            Console.WriteLine("=== PRODUÇÕES CADASTRADAS ===");

            var producoes = new Production("0", 0).Listar();

            if (producoes.Count == 0)
            {
                Console.WriteLine("Nenhuma produção cadastrada.");
                return;
            }

            foreach (var producao in producoes)
            {
                Console.WriteLine(producao);
                Console.WriteLine("----------------------------");
            }
        }

        static void Localizar()
        {
            Console.WriteLine("=== LOCALIZAR PRODUÇÃO ===");

            Console.Write("Digite o ID da produção: ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("ID inválido.");
                return;
            }

            try
            {
                Production teste = new Production("0", 0);
                string resultado = teste.Localizar(id);

                Console.WriteLine();
                Console.WriteLine(resultado);
            }
            catch (NullReferenceException)
            {
                Console.WriteLine("Produção não encontrada.");
            }
        }

        static void Excluir()
        {
            Console.WriteLine("=== EXCLUIR PRODUÇÃO ===");

            Console.Write("Digite o ID da produção: ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("ID inválido.");
                return;
            }

            Production teste = new Production("0", 0);

            if (teste.Delete(id))
            {
                Console.WriteLine("Produção excluída com sucesso!");
            }
            else
            {
                Console.WriteLine("Produção não encontrada.");
            }
        }

        static void TestarQuantidade()
        {
            Console.WriteLine("=== TESTE DO LIMITE DE QUANTIDADE ===");

            try
            {
                Production p = new Production("1", 1000.00f);

                Console.WriteLine("Produção criada:");
                Console.WriteLine(p);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine("Teste realizado com sucesso!");
                Console.WriteLine("A quantidade acima de 999.99 foi rejeitada.");
                Console.WriteLine(e.Message);
            }
        }

        static void TestarId()
        {
            Console.WriteLine("=== TESTE DO LIMITE DE IDs ===");

            try
            {
                Production p = new Production(99999, DateTime.Today, "1", 10);

                Console.WriteLine("Produção criada:");
                Console.WriteLine(p);
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("Limite de IDs atingido!");
                Console.WriteLine(e.Message);
            }
        }
    }
}


