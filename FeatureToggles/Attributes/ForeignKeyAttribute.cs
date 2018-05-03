using System;

namespace FeatureToggle.Attributes
{
    class ForeignKeyAttribute : Attribute
    {
        public Type ReferenceType { get; set; }

        public ForeignKeyAttribute(Type referenceType)
        {
            ReferenceType = referenceType;
        }
    }
}
