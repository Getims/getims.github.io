using System.Collections.Generic;
using Project.Scripts.Configs;
using Project.Scripts.UI.Common.Panels;
using UnityEngine;
using Zenject;

namespace Project.Scripts.UI.Common
{
    public interface IUIFactory
    {
        TElement Create<TElement>(TElement prefab) where TElement : MonoBehaviour;

        TElement Create<TElement>(TElement prefab, Transform parent, bool withComponent = true)
            where TElement : MonoBehaviour;

        TPanel Create<TPanel>() where TPanel : UIPanel;
        TPanel Create<TPanel>(Transform parent) where TPanel : UIPanel;
        TPanel GetPanel<TPanel>(bool createIfNot = true) where TPanel : UIPanel;
    }

    public class UIFactory : IUIFactory
    {
        private readonly DiContainer _diContainer;
        private readonly Transform _uiContainer;

        private Dictionary<string, UIPanel> _uiPanels = new Dictionary<string, UIPanel>();
        private Dictionary<string, UIPanel> _createdPanels = new Dictionary<string, UIPanel>();

        public UIFactory(DiContainer diContainer)
        {
            _diContainer = diContainer;
        }

        public UIFactory(DiContainer diContainer, UIConfig uiConfig, Transform uiContainer = null)
        {
            _diContainer = diContainer;
            _uiContainer = uiContainer;
            SetupDictionary(uiConfig.Prefabs);
        }

        public TElement Create<TElement>(TElement prefab) where TElement : MonoBehaviour =>
            CreateElement(prefab, null);

        public TElement Create<TElement>(TElement prefab, Transform parent, bool withComponent = true)
            where TElement : MonoBehaviour =>
            CreateElement(prefab, parent, withComponent);

        private TElement CreateElement<TElement>(TElement prefab, Transform parent, bool withComponent = true)
            where TElement : MonoBehaviour
        {
            if (parent == null)
            {
                if (withComponent)
                    return _diContainer.InstantiatePrefabForComponent<TElement>(prefab);
                return _diContainer.InstantiatePrefab(prefab).GetComponent<TElement>();
            }

            if (withComponent)
                return _diContainer.InstantiatePrefabForComponent<TElement>(prefab, parent);

            return _diContainer.InstantiatePrefab(prefab, parent).GetComponent<TElement>();
        }

        public TPanel Create<TPanel>() where TPanel : UIPanel =>
            CreatePanel<TPanel>(null);

        public TPanel Create<TPanel>(Transform parent) where TPanel : UIPanel =>
            CreatePanel<TPanel>(parent);

        public TPanel GetPanel<TPanel>(bool createIfNot = true) where TPanel : UIPanel
        {
            string type = typeof(TPanel).Name;
            _createdPanels.TryGetValue(type, out UIPanel panel);
            if (panel == null && createIfNot)
                panel = CreatePanel<TPanel>(null);

            return (TPanel)panel;
        }

        private TPanel CreatePanel<TPanel>(Transform parent) where TPanel : UIPanel
        {
            string type = typeof(TPanel).Name;
            TPanel panelPrefab = GetPanelPrefab<TPanel>(type);
            if (panelPrefab == null)
                LogPanelNotFound(type);

            parent = parent == null ? _uiContainer : parent;
            TPanel createdPanel;
            if (parent == null)
                createdPanel = _diContainer.InstantiatePrefabForComponent<TPanel>(panelPrefab);
            else
                createdPanel = _diContainer.InstantiatePrefabForComponent<TPanel>(panelPrefab, parent);

            SavePanel(type, createdPanel);
            return createdPanel;
        }

        private void SetupDictionary(List<UIPanel> uiList)
        {
            foreach (var uiPanel in uiList)
            {
                string type = uiPanel.GetType().Name;
                AddPanel(type, uiPanel);
            }
        }

        private void AddPanel(string type, UIPanel uiPanel)
        {
            _uiPanels.TryAdd(type, uiPanel);
        }

        private void SavePanel(string type, UIPanel uiPanel)
        {
            _createdPanels.TryAdd(type, uiPanel);
        }

        private TPanel GetPanelPrefab<TPanel>(string type) where TPanel : UIPanel
        {
            _uiPanels.TryGetValue(type, out UIPanel panelPrefab);
            return (TPanel)panelPrefab;
        }

        private void LogPanelNotFound(string panelType)
        {
            string errorLog = $"Data of type ({panelType} not found!";
            Debug.LogError(errorLog);
        }
    }
}