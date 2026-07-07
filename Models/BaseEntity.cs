using library_system.Interfaces;

namespace library_system.Models
{
    public abstract class BaseEntity : IEntity
    {
        public int Id { get; set; }
    }
}
