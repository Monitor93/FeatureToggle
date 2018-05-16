using System;
namespace FeatureToggle.Attributes
{
    /// <summary>
    /// Аттрибут для создания автоинкрементного первичного ключа
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    class IdentityAttribute : Attribute
    {
    }
}
