using FeatureToggle.DataBase.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FeatureToggle.TransferObjects
{
    /// <summary>
    /// Контекст фичи
    /// </summary>
    public class FeatureContextDto
    {
        /// <summary>
        /// Имя контекста
        /// </summary>
        public string ContextName { get; set; }

        /// <summary>
        /// Параметры контекста
        /// </summary>
        public Dictionary<string,bool> Params { get; set; }

        /// <summary>
        /// Активна ли фича в данном контексте по параметру <c>param</c>
        /// </summary>
        /// <param name="param">Значение контекста</param>
        /// <returns>Активна ли фича</returns>
        public bool? this[string param]
        {
            get
            {
                return Params.ContainsKey(param)
                    ? Params[param]
                    : (bool?)null;
            }
        }

        public FeatureContextDto(string contextName, Dictionary<string, bool> values)
        {
            ContextName = contextName;
            Params = values;
        }

        internal FeatureContextDto(string contextName, List<FeatureToContext> context)
        {
            ContextName = contextName;
            Params = context
                .Where(x => x.Context == contextName)
                .ToDictionary(x => x.Param, x => x.IsEnable);
        }
    }
}
