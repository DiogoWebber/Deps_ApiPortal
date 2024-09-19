using Deps_CleanArchitecture.Core.Enums;
using Deps_CleanArchitecture.SharedKernel.Extensions;
using System;
using Xunit;

namespace Deps_CleanArchitecture.UnitTests.SharedKernel.Extensions.EnumExtensionsTests
{
    public class Description
    {
        [Theory]
        [InlineData(PerfilUsuario.Automacao, "Automação")]
        [InlineData(PerfilUsuario.AdministradorPortal, "Administrador do portal")]
        [InlineData(PerfilUsuario.UsuarioGestor, "Usuário gestor")]
        [InlineData(PerfilUsuario.GerenteComercial, "Gerente comercial")]
        public void CanGetDescription(Enum value, string expected)
        {
            var result = value.Description();

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(InternalErrorCode.VersaoApiNaoInformada, "Description Not Found")]
        public void CanNotGetDescription(Enum value, string expected)
        {
            var result = value.Description();

            Assert.Equal(expected, result);
        }
    }
}
