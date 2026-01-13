using System;
using UnityEngine;

namespace Project.Scripts.UI.Common
{
    [Serializable]
    public class UIContainerProvider
    {
        [SerializeField]
        private Transform _menuContainer;

        [SerializeField]
        private Transform _windowsContainer;

        public Transform MenuContainer => _menuContainer;
        public Transform WindowsContainer => _windowsContainer;
    }
}