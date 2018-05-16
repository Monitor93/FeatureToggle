using System;
using Castle.Windsor;
using TestConsole.Manager;
using System.Threading.Tasks;
using FeatureToggle.TransferObjects;
using System.Collections.Generic;

namespace TestConsole
{
    class Program
    {
        /// <summary>
        /// IoC контейнер
        /// </summary>
        static readonly WindsorContainer Container;

        private static ISomeManager _someManager;

        static void Main(string[] args)
        {
            _someManager = Container.Resolve<ISomeManager>();
            MainProccess();
            MainProcessWithContext();
            ParallelMainProcesses();
            ParallelTest();
            ParallelTestAsTask();
            SqlInjection();
            Console.ReadKey();
        }

        /// <summary>
        /// Проверка основных функций <see cref="FeatureToggle.FeatureToggleService"/>
        /// </summary>
        static void MainProccess()
        {
            Console.WriteLine("================ MainProccess:");
            _someManager.AddFeature("falseFeature", false);
            _someManager.AddFeature("trueFeature", true);
            _someManager.AddFeature("changedFeature", true);
            _someManager.GetFeature("falseFeature");
            _someManager.GetFeature("trueFeature");
            _someManager.GetFeature("changedFeature");
            _someManager.GetFeature("not exists feature");
            _someManager.AddFeature("changedFeature", false);
            _someManager.GetFeature("changedFeature");
            try
            {
                _someManager.CheakAndGetFeature("falseFeature");
                _someManager.CheakAndGetFeature("trueFeature");
                _someManager.CheakAndGetFeature("not exists feature");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            _someManager.AddFeature("featureForDelete", true);
            _someManager.GetFeature("featureForDelete");
            _someManager.DeleteFeature("featureForDelete");
            _someManager.GetFeature("featureForDelete");
            try
            {
                _someManager.CheakAndGetFeature("featureForDelete");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Проверка основных функций <see cref="FeatureToggle.FeatureToggleService"/>
        /// </summary>
        static void MainProcessWithContext()
        {
            Console.WriteLine("================ MainProcessWithContext:");
            _someManager.AddFeatureWithContext("featureWithContext", false, new List<FeatureContextDto>
            {
                new FeatureContextDto("OS", new Dictionary<string, bool>
                {
                    ["Linux"] = false,
                    ["Windows"] = true,
                }),
                new FeatureContextDto("ContextForDelete", new Dictionary<string, bool>
                {
                    ["Param"] = true,
                    ["ParamToDelete"] = true,
                })
            });
            _someManager.GetFeature("featureWithContext");
            _someManager.GetFeature("featureWithContext", "OS", "Linux");
            _someManager.GetFeature("featureWithContext", "OS", "Windows");
            _someManager.GetFeature("featureWithContext", "OS", "MacOS");
            _someManager.GetFeature("featureWithContext", "ContextForDelete", "Param");
            _someManager.GetFeature("featureWithContext", "ContextForDelete", "ParamToDelete");
            _someManager.DeleteContext("ContextForDelete", "featureWithContext", "ParamToDelete");
            _someManager.GetFeature("featureWithContext", "ContextForDelete", "Param");
            _someManager.GetFeature("featureWithContext", "ContextForDelete", "ParamToDelete");
            _someManager.DeleteContext("ContextForDelete", "featureWithContext");
            _someManager.GetFeature("featureWithContext", "ContextForDelete", "Param");
        }

        /// <summary>
        /// Проверка работы основных процессов в паралельных потоках
        /// </summary>
        static void ParallelMainProcesses()
        {
            Console.WriteLine("================ ParallelMainProcesses:");
            Parallel.Invoke(
                MainProccess,
                MainProcessWithContext);
        }

        /// <summary>
        /// Тест работы в паралельных потоках
        /// </summary>
        static void ParallelTest()
        {
            Console.WriteLine("================ ParallelTest:");
            Parallel.Invoke(
                () => ParralelProccessWrite(true),
                ParralelProccessRead,
                () => ParralelProccessWrite(false),
                ParralelProccessRead,
                ParralelProccessCheckRead,
                ParralelProccessDelete,
                () => ParralelProccessWrite(false),
                () => ParralelProccessWrite(true),
                ParralelProccessRead,
                ParralelProccessRead,
                ParralelProccessCheckRead,
                ParralelProccessDelete);
        }

        /// <summary>
        /// Тест паралельной работы через таски
        /// </summary>
        static void ParallelTestAsTask()
        {
            Console.WriteLine("================ ParallelTestAsTask:");
            Task.WaitAll(
                Task.Factory.StartNew(() => ParralelProccessWrite(true)),
                Task.Factory.StartNew(ParralelProccessRead),
                Task.Factory.StartNew(() => ParralelProccessWrite(false)),
                Task.Factory.StartNew(ParralelProccessRead),
                Task.Factory.StartNew(ParralelProccessCheckRead),
                Task.Factory.StartNew(ParralelProccessDelete),
                Task.Factory.StartNew(() => ParralelProccessWrite(false)),
                Task.Factory.StartNew(() => ParralelProccessWrite(true)),
                Task.Factory.StartNew(ParralelProccessRead),
                Task.Factory.StartNew(ParralelProccessRead),
                Task.Factory.StartNew(ParralelProccessCheckRead),
                Task.Factory.StartNew(ParralelProccessDelete));
        }

        /// <summary>
        /// Функция создания фичи для паралельного теста
        /// </summary>
        /// <param name="featureValue">Значение фичи</param>
        static void ParralelProccessWrite(bool featureValue)
        {
            var someManager = Container.Resolve<ISomeManager>();
            someManager.AddFeature("parallelFeature", featureValue);
        }

        /// <summary>
        /// Функция чтение фичи для паралельного теста
        /// </summary>
        static void ParralelProccessRead()
        {
            var someManager = Container.Resolve<ISomeManager>();
            someManager.GetFeature("parallelFeature");
        }

        /// <summary>
        /// Функция удаления фичи для паралельного теста
        /// </summary>
        static void ParralelProccessDelete()
        {
            var someManager = Container.Resolve<ISomeManager>();
            someManager.DeleteFeature("parallelFeature");
        }

        /// <summary>
        /// Функция чтение фичи с проверкой для паралельного теста
        /// </summary>
        static void ParralelProccessCheckRead()
        {
            var someManager = Container.Resolve<ISomeManager>();
            try
            {
                someManager.CheakAndGetFeature("not exists feature");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Проверка sql инекций
        /// </summary>
        static void SqlInjection()
        {
            Console.WriteLine("================ SqlInjection:");
            _someManager.GetFeature("'a'; insert Feature (Id,Value) Values ('SqlInjection', 1);");
            _someManager.GetFeature("SqlInjection");

        }

        /// <summary>
        /// Статический конструктор для инициализации IoC контейнера
        /// </summary>
        static Program()
        {
            Container = new WindsorContainer();

            Container.Install(new ManagerInstaller(), new FeatureToggleInstaller());
        }
    }
}
