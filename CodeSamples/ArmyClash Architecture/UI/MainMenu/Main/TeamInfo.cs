using Project.Scripts.Runtime.Enums;
using TMPro;
using UnityEngine;

namespace Project.Scripts.UI.MainMenu.Main
{
    public class TeamInfo : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _teamName;

        [SerializeField]
        private TMP_Text _winsCount;

        public void UpdateInfo(UnitTeam unitTeam, int count)
        {
            _teamName.text = $"{unitTeam}:";
            _winsCount.text = $"Wins count: {count}";
        }
    }
}