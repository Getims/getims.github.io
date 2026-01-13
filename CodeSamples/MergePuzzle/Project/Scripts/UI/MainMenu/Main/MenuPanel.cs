using System;
using Project.Scripts.UI.Common.Panels;
using Project.Scripts.UI.MainMenu.Enums;
using Project.Scripts.UI.MainMenu.Navigation;
using UnityEngine;

namespace Project.Scripts.UI.MainMenu.Main
{
    public class MenuPanel : UIPanel
    {
        [SerializeField]
        private MenuNavigationPanel _menuNavigationPanel;

        public event Action OnCollectionOpenRequest;
        public event Action OnSettingsOpenRequest;
        public event Action OnStartLevelOpenRequest;

        private void Start()
        {
            _menuNavigationPanel.OnControlButtonClick += OnControlButtonClick;
            _menuNavigationPanel.Initialize();
        }

        private void OnControlButtonClick(MenuNavigationState state)
        {
            switch (state)
            {
                case MenuNavigationState.MainMenu:
                    break;

                case MenuNavigationState.Collection:
                    OnCollectionOpenRequest?.Invoke();
                    break;

                case MenuNavigationState.Settings:
                    OnSettingsOpenRequest?.Invoke();
                    break;

                case MenuNavigationState.StartLevel:
                    OnStartLevelOpenRequest?.Invoke();
                    break;
            }
        }
    }
}