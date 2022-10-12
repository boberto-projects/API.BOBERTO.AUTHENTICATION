using api_authentication_boberto.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_authentication_boberto.Routes
{
    public static class UsuarioRoute
    {
        public static void AdicionarUsuarioRoute(this WebApplication app)
        {
            app.MapGet("/perfil", [Authorize] ([FromServices] IUsuarioService usuarioLogado) =>
            {
                return usuarioLogado.ObterUsuarioLogado();
            });
        }
    }
}
