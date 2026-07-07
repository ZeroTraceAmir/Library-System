namespace library_system.Services
{
    public abstract class BaseService<T>
    {
        protected abstract void Validate(T entity);
    }
}
//chon logic validation baraye har entity fargh mikone az abstract estefade kardim
// protected ham yani baraye child hash dar dastreses vali be sorate public baraye hame dar dastras nist.
