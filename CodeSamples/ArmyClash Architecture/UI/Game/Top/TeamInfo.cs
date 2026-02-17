using Project.Scripts.Runtime.Enums;
using TMPro;
using UnityEngine;

namespace Project.Scripts.UI.Game.Top
{
    public class TeamInfo : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _teamName;

        [SerializeField]
        private TMP_Text _unitsCount;

        private UnitTeam _unitTeam;

        public UnitTeam UnitTeam => _unitTeam;

        public void Setup(UnitTeam unitTeam)
        {
            _unitTeam = unitTeam;
            _teamName.text = unitTeam.ToString();
        }

        public void UpdateInfo(int count)
        {
            _unitsCount.text = count.ToString();
        }
    }
}