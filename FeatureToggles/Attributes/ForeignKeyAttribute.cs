using System;

namespace FeatureToggle.Attributes
{
    /// <summary>
    /// Аттрибут для внешних ключей
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    class ForeignKeyAttribute : Attribute
    {
        /// <summary>
        /// Тип сущности внешнего ключа
        /// </summary>
        public Type ReferenceType { get; set; }

        /// <summary>
        /// создаёт новый экземпляр аттрибута
        /// </summary>
        /// <param name="referenceType">Тип для внешнего ключа</param>
        public ForeignKeyAttribute(Type referenceType)
        {
            ReferenceType = referenceType;
        }
    }
}
