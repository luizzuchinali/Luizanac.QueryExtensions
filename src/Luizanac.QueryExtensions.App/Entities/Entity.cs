using System;

namespace Luizanac.QueryExtensions.App.Entities
{
    public class Entity
    {
        public Guid Id { get; private set; }
        public Entity()
        {
            Id = Guid.NewGuid();
        }
    }
}