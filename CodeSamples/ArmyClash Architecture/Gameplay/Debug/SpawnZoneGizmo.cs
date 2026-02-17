using System.Collections.Generic;
using Project.Scripts.Gameplay.Factory.Utils;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Project.Scripts.Gameplay.Debug
{
    public class SpawnZoneGizmo : MonoBehaviour
    {
        [SerializeField]
        private Color _gizmoColor = new Color(0f, 1f, 0f, 0.25f);

        [Title("Spawn points")]
        [SerializeField]
        private Color _pointColor = new Color(1f, 0f, 0f, 0.5f);

        [SerializeField]
        private float _pointSize = 1f;

        private List<Vector3> _points = new List<Vector3>();

        private void OnDrawGizmos()
        {
            Gizmos.color = _gizmoColor;
            Gizmos.DrawCube(transform.position, transform.localScale);
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, transform.localScale);

            if (_points == null || _points.Count == 0)
                return;

            Gizmos.color = _pointColor;

            for (var i = 0; i < _points.Count; i++)
            {
                var point = _points[i];
                Gizmos.DrawWireSphere(point, _pointSize / 2);
                var style = new GUIStyle { normal = { textColor = _pointColor } };
                Handles.Label(point + Vector3.up * 0.6f, (i + 1).ToString(), style);
            }
        }

        [Button]
        private void SamplePoints(int count)
        {
            Vector3 center = transform.position;
            Vector3 size = transform.localScale;

            count = count > 0 ? count : 1;

            _points = SpawnZoneHelper.DistributeSquares(center, size, count);
        }
    }
}