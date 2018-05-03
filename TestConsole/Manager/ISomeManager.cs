using FeatureToggle.TransferObjects;
using System.Collections.Generic;

namespace TestConsole.Manager
{
    public interface ISomeManager
    {
        void AddFeature(string key, bool value);
        void GetFeature(string key);
        void GetFeature(string key, string context, string param);
        void CheakAndGetFeature(string key);
        void AddFeatureWithContext(string key, bool value, List<FeatureContextDto> contexts);
        void DeleteFeature(string key);
        void DeleteContext(string context, string feature);
        void DeleteContext(string context, string feature, string param);
    }
}
