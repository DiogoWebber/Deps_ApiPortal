using Deps_CleanArchitecture.Core.Entities.PerfilAggregate;
using Xunit;

namespace Deps_CleanArchitecture.UnitTests.Core.Entities.PerfilEntity
{
    public class PerfilInativar
    {
        private Perfil CreatePerfil()
        {
            return Perfil.Factory.NovoPerfil("teste", 1);
        }

        [Fact]
        public void Inativar()
        {
            var perfil = CreatePerfil();
            perfil.Inativar();

            Assert.True(!perfil.Ativo);
        }
    }
}
