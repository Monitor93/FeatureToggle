using System;
using FeatureToggle.Enums;
using FeatureToggle.TransferObjects;
using System.Collections.Generic;
using System.Threading;
using FeatureToggle;

namespace TestConsole.Manager
{
    class SomeManager : ISomeManager
    {
        private readonly IFeatureToggle _featureToggle;

        public SomeManager(IFeatureToggle featureToggle)
        {
            _featureToggle = featureToggle;
        }

        public void AddFeature(string key, bool value)
        {
            _featureToggle.CreateOrUpdateFeature(new FeatureDto { Key = key, Value = value });
            WriteInConsoleByThread(string.Format("Create {0} is {1}", key, value));
        }

        public void GetFeature(string key)
        {
            WriteInConsoleByThread(string.Format("Get {0}: {1}", key, _featureToggle.IsEnable(key)));
        }

        public void CheakAndGetFeature(string key)
        {
            WriteInConsoleByThread(string.Format("Get {0}: {1}", key, _featureToggle.IsEnable(key, DefaultFeatureValue.Exception)));
        }

        public void AddFeatureWithContext(string key, bool value, List<FeatureContextDto> contexts)
        {
            _featureToggle.CreateOrUpdateFeature(new FeatureDto(key, value, contexts));
            WriteInConsoleByThread(string.Format("Create {0} is {1} withContexts", key, value));
        }

        public void GetFeature(string key, string context, string param)
        {
            WriteInConsoleByThread(string.Format("Get {0} {1} {2}: {3}", key, context, param, _featureToggle.IsEnable(key, context, param)));
        }

        public void DeleteFeature(string key)
        {
            _featureToggle.DeleteFeature(key);
            WriteInConsoleByThread(string.Format("Feature {0} is delete", key));
        }

        public void DeleteContext(string context, string feature)
        {
            _featureToggle.DeleteContext(context, feature);
            WriteInConsoleByThread(string.Format("Context {0} in {1} is delete", context, feature));
        }
        public void DeleteContext(string context, string feature, string param)
        {
            _featureToggle.DeleteContext(context, feature, param);
            WriteInConsoleByThread(string.Format("{0} in {1} for {2} is delete", param, context, feature));
        }

        private void WriteInConsoleByThread(string str)
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            Console.ForegroundColor = (ConsoleColor)((threadId % 15) + 1);
            Console.WriteLine("Tread: {0}\t{1}", threadId, str);
        }
    }
}
