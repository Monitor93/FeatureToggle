using System;

namespace FeatureToggle.Exceptions
{
    /// <summary>
    /// Представление ошибки когда заданная фича была не найдена
    /// </summary>
    public class FeatureNotFounException : Exception
    {
        public FeatureNotFounException(string featureKey) : base($"Не удалось найти сведенья о функциональности \"{featureKey}\"") { }
    }
}
