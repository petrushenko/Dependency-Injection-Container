using System;

namespace DependencyInjectionContainer
{
    public class Dependency
    {
        public Type Type { get; }

        public LifeType LifeType { get; }

        public object Key { get; }

        public object Instance { get; set; }

        public Dependency(Type type, LifeType lifeType, object key)
        {
            Key = key;
            Type = type;
            LifeType = lifeType;
        }
    }
}
