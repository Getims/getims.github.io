using System;
using Project.Scripts.Core.Constants;

namespace Project.Scripts.Infrastructure.Configs
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ConfigCategoryAttribute : Attribute
    {
        public ConfigCategory Category { get; }

        public ConfigCategoryAttribute(ConfigCategory category)
        {
            Category = category;
        }
    }
}