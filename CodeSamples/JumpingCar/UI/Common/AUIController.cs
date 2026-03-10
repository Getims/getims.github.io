using System.Collections.Generic;
using Project.Scripts.UI.Common.Panels;
using UnityEngine;

namespace Project.Scripts.UI.Common
{
    public abstract class AUIController : MonoBehaviour
    {
        [SerializeField]
        private List<UIPanel> _panels = new List<UIPanel>();

        private Dictionary<string, UIPanel> _uiPanels = new Dictionary<string, UIPanel>();

        public T GetPanel<T>() where T : UIPanel
        {
            string type = typeof(T).Name;
            _uiPanels.TryGetValue(type, out UIPanel panel);

            if (panel == null)
            {
                Debug.LogError($"Panel of type {type} not found!");
                return null;
            }

            return (T)panel;
        }

        public T ShowPanel<T>() where T : UIPanel
        {
            T panel = GetPanel<T>();
            if (panel == null)
                return null;

            panel.Initialize();
            panel.Show();

            return panel;
        }

        protected virtual void Start()
        {
            SetupDictionary(_panels);
        }

        protected virtual void OnDestroy()
        {
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
    }
}