using System;
using Project.Scripts.Modules.DebugModule.Core;
using TMPro;
using UnityEngine;

namespace Project.Scripts.Modules.DebugModule.UI.Elements
{
    public class CheatGroupSeparator : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _title;

        public void Initialize(CheatGroupType groupType)
        {
            switch (groupType)
            {
                case CheatGroupType.Base:
                    _title.text = "";
                    break;
                case CheatGroupType.Gameplay:
                    _title.text = "Gameplay";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(groupType), groupType, null);
            }
        }
    }
}