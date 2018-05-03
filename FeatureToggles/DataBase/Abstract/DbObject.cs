namespace FeatureToggle.DataBase.Abstract
{
    abstract class DbObject<T>
    {
        public virtual T Id { get; set; }

        protected DbObject() { }
    }
}
