using Castle.Windsor;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using FeatureToggle;

namespace TestConsole
{
    class FeatureToggleInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IFeatureToggle>().ImplementedBy<FeatureToggleService>().LifeStyle.Transient);
        }
    }
}
