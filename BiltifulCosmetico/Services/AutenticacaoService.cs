using Biltiful.ConsoleApp.Models;
using Biltiful.ConsoleApp.Repositories;
namespace Biltiful.ConsoleApp.Services;

public class AutenticacaoService
{
    private readonly UsuarioRepositorio _repo;
    public AutenticacaoService(UsuarioRepositorio repo) => _repo = repo;
    public Usuario? Autenticar(string login, string senha)
    {
        if (string.IsNullOrWhiteSpace(login) || string.IsNullOrEmpty(senha))
            return null;
        var u = _repo.LocalizarPorLogin(login);
        return u is not null && u.Situacao == 'A' && u.Senha == senha ? u : null;
    }
}
