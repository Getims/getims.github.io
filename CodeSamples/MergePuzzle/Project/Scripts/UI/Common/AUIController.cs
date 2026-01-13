using System;
using System.Collections.Generic;
using Project.Scripts.UI.Common.Panels;
using UnityEngine;
using Zenject;

namespace Project.Scripts.UI.Common
{
    public abstract class AUIController : MonoBehaviour
    {
        [Inject] private IUIFactory _iuiFactory;
        [Inject] private UIContainerProvider _uiContainerProvider;

        private List<UIPanel> _popups = new List<UIPanel>();

        public virtual void Initialize()
        {
        }

        protected virtual void Start()
        {
        }

        protected virtual void OnDestroy()
        {
            Resources.UnloadUnusedAssets();
        }

        protected void CloseAllPopups()
        {
            foreach (UIPanel popup in _popups)
            {
                if (popup != null)
                    popup.Hide();
            }

            _popups.Clear();
        }

        protected T OpenPopup<T>(Action closeClick, Action claimClick) where T : PopupPanel
        {
            T popup = _iuiFactory.Create<T>(_uiContainerProvider.WindowsContainer);
            _popups.Add(popup);
            popup.Initialize();
            popup.Show();
            if (closeClick != null)
                popup.OnCloseClick += closeClick;
            if (claimClick != null)
                popup.OnClaimClick += claimClick;

            return popup;
        }

        protected T OpenWindow<T>() where T : UIPanel
        {
            T window = _iuiFactory.Create<T>(_uiContainerProvider.MenuContainer);
            _popups.Add(window);
            window.Initialize();
            window.Show();

            return window;
        }

        protected T ShowPanel<T>() where T : UIPanel
        {
            T panel = _iuiFactory.Create<T>(_uiContainerProvider.MenuContainer);
            panel.Initialize();
            panel.Show();

            return panel;
        }
    }
}