using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using TestConsole.Manager;

namespace TestConsole
{
    public class ManagerInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<ISomeManager>().ImplementedBy<SomeManager>().LifeStyle.Transient);
        }
    }
}
