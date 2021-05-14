using System;

namespace Totem
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class QueueNameAttribute : Attribute
    {
        public QueueNameAttribute(string name) =>
            Name = name ?? throw new ArgumentNullException(nameof(name));

        public string Name { get; }
    }
}