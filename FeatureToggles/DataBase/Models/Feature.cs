using FeatureToggle.DataBase.Abstract;
using FeatureToggle.TransferObjects;

namespace FeatureToggle.DataBase.Models
{
    class Feature : DbObject<string>
    {
        public bool Value { get; set; }

        public Feature() { }
        public Feature(FeatureDto dto)
        {
            Id = dto.Key;
            Value = dto.Value;
        }
    }
}