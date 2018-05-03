using FeatureToggle.Attributes;
using FeatureToggle.DataBase.Abstract;

namespace FeatureToggle.DataBase.Models
{
    class FeatureToContext : DbObject<int>
    {
        [Identity]
        public override int Id { get; set; }

        [ForeignKey(typeof(Feature))]
        public string Feature { get; set; }

        [ForeignKey(typeof(Context))]
        public string Context { get; set; }

        public string Param { get; set; }

        public bool IsEnable { get; set; }
    }
}
