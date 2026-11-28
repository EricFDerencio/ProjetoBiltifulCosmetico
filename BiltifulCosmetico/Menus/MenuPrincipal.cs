using Biltiful.ConsoleApp.Models;
using Biltiful.ConsoleApp.Services;
namespace Biltiful.ConsoleApp.Menus;

public class MenuPrincipal
{
    private readonly AutorizacaoService _auth;
    public MenuPrincipal(AutorizacaoService auth) => _auth = auth;
    public void Exibir(Usuario usuario)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== BILTIFUL - MENU ===");
            Console.WriteLine($"Usuário: {usuario.Nome} | Perfil: {usuario.Perfil}");
            Console.WriteLine("1 - Administração\n2 - Produção\n3 - Vendas\n4 - Compras\n0 - Logout");
            Console.Write("Opção: ");
            var op = Console.ReadLine() ?? "";
            if (op == "0")
                return;
            string? m = op switch
            {
                "1" => Modulos.Administracao,
                "2" => Modulos.Producao,
                "3" => Modulos.Vendas,
                "4" => Modulos.Compras,
                _ => null
            };
            if (m is null)
            {
                Pausar("Opção inválida.");
                continue;
            }
            if (!_auth.PodeAcessar(usuario, m))
            {
                Pausar("ACESSO NEGADO. O usuário atual não possui permissão.");
                continue;
            }
            Pausar($"Módulo {m} iniciado. Integre aqui as funcionalidades do grupo.");
        }
    }
    private static void Pausar(string t)
    {
        Console.WriteLine($"\n{t}\nPressione ENTER para continuar.");
        Console.ReadLine();
    }
}
