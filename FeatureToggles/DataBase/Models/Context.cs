using FeatureToggle.DataBase.Abstract;

namespace FeatureToggle.DataBase.Models
{
    class Context : DbObject<string>
    {
        public Context() { }

        public Context(string name)
        {
            Id = name;
        }
    }
}
