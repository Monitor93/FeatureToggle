using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using TestConsole.Manager;

namespace TestConsole
{
    public class ManagerInstaller : IWindsorInstaller
    {
        /// <summary>
        /// Инсталер IoC контейнера для <see cref="ISomeManager"/>
        /// </summary>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<ISomeManager>().ImplementedBy<SomeManager>().LifeStyle.Transient);
        }
    }
}
