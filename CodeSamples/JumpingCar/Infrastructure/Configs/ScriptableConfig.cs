using System;
using Project.Scripts.Runtime.Constants;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Scripts.Infrastructure.Configs
{
    public abstract class ScriptableConfig : ScriptableObject
    {
        [TitleGroup("Info")]
        [BoxGroup("Info/In", showLabel: false)]
        public string FileName = "Config";

#if UNITY_EDITOR
        public string GetConfigCategory()
        {
            var attr = (ConfigCategoryAttribute)Attribute.GetCustomAttribute(
                GetType(), typeof(ConfigCategoryAttribute));

            return attr != null
                ? attr.Category.ToString()
                : ConfigCategory.Core.ToString();
        }
#endif
    }
}