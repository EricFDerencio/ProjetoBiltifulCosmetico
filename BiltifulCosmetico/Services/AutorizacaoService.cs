using Biltiful.ConsoleApp.Constants;
using Biltiful.ConsoleApp.Models;
namespace Biltiful.ConsoleApp.Services;

public class AutorizacaoService
{
    public bool PodeAcessar(Usuario usuario, string modulo)
    {
        if (usuario is null || usuario.Situacao != 'A')
            return false;
        if (usuario.Perfil == Perfis.Admin)
            return true;
        return modulo.ToUpperInvariant()
            switch
        {
            Modulos.Producao => usuario.Perfil == Perfis.Producao,
            Modulos.Vendas => usuario.Perfil == Perfis.Vendas,
            Modulos.Compras => PodeAcessarCompras(usuario),
            Modulos.Administracao => false,
            _ => false
        };
    }
    private static bool PodeAcessarCompras(Usuario usuario)
    {
        return usuario.Situacao == 'A' ||
            usuario.Perfil == Perfis.Compras ||
            usuario.Perfil == Perfis.Admin;
    }
}
