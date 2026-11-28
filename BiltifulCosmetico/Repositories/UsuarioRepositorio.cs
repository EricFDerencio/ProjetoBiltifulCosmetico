using Biltiful.ConsoleApp.Models;
namespace Biltiful.ConsoleApp.Repositories;

public class UsuarioRepositorio
{
    private readonly string _arquivo;
    public UsuarioRepositorio(string arquivo) => _arquivo = arquivo;
    public List<Usuario> Listar()
    {
        var lista = new List<Usuario>();
        if (!File.Exists(_arquivo))
            return lista;
        try
        {
            foreach (var linha in File.ReadLines(_arquivo))
            {
                if (string.IsNullOrWhiteSpace(linha))
                    continue;
                var c = linha.Split(';');
                if (c.Length != 6 || !int.TryParse(c[0], out int id) || c[5].Trim().Length != 1)
                    continue;
                lista.Add(new Usuario { Id = id, Nome = c[1].Trim(), Login = c[2].Trim(), Senha = c[3], Perfil = c[4].Trim().ToUpperInvariant(), Situacao = char.ToUpperInvariant(c[5].Trim()[0]) });
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new InvalidOperationException("Sem permissão para acessar o arquivo de usuários.", ex);
        }
        catch (IOException ex)
        {
            throw new InvalidOperationException("Erro ao ler o arquivo de usuários.", ex);
        }
        return lista;
    }
    public Usuario? LocalizarPorLogin(string login) =>
        Listar().FirstOrDefault(u => u.Login.Equals(login.Trim(),
            StringComparison.OrdinalIgnoreCase));
}
