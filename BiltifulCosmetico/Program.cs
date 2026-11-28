using Biltiful.ConsoleApp.Menus;
using Biltiful.ConsoleApp.Repositories;
using Biltiful.ConsoleApp.Services;

string arquivo = Path.Combine(AppContext.BaseDirectory, "Data", "Usuarios.dat");

var auth = new AutenticacaoService(new UsuarioRepositorio(arquivo));
var menu = new MenuPrincipal(new AutorizacaoService());

while (true)
{
    Console.Clear();
    Console.WriteLine("=== BILTIFUL ===\nDigite SAIR no login para encerrar.\n");
    Console.Write("Login: ");
    var login = Console.ReadLine() ?? "";

    if (login.Equals("SAIR", StringComparison.OrdinalIgnoreCase))
        break;

    Console.Write("Senha: ");
    var senha = Console.ReadLine() ?? "";

    try
    {
        var u = auth.Autenticar(login, senha);
        if (u is null)
        {
            Console.WriteLine("Login ou senha inválidos.\nPressione ENTER.");
            Console.ReadLine();
            continue;
        }
        menu.Exibir(u);
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine(ex.Message);
        Console.ReadLine();
    }
}
