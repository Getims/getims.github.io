using System;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Data;
using Project.Scripts.UI.Common.Panels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Project.Scripts.UI.MainMenu.Main
{
    public class MenuPanel : UIPanel
    {
        [SerializeField]
        private Button _startButton;

        [SerializeField]
        private TMP_Text _battlesCount;

        [SerializeField]
        private TeamInfo _teamInfoPrefab;

        [SerializeField]
        private Transform _container;

        [Inject] private IGameDataService _gameDataService;

        private List<TeamInfo> _teamInfos = new List<TeamInfo>();

        public event Action OnStartLevelOpenRequest;

        public override void Show()
        {
            base.Show();
            UpdateBattlesCount();
            UpdateTeamsInfo();
        }

        protected void Start()
        {
            _startButton.onClick.AddListener(OnStartButtonClick);
        }

        protected override void OnDestroy()
        {
            _startButton.onClick.RemoveListener(OnStartButtonClick);
        }

        private void UpdateBattlesCount()
        {
            _battlesCount.text = $"Battles count: {_gameDataService.BattlesCount.Value}";
        }

        private void UpdateTeamsInfo()
        {
            var teamsInfo = _gameDataService.TeamsInfo;

            for (int i = 0; i < teamsInfo.Count; i++)
            {
                var teamInfo = teamsInfo.ElementAt(i);
                if (i < _teamInfos.Count)
                {
                    _teamInfos[i].UpdateInfo(teamInfo.UnitTeam, teamInfo.WinsCount);
                    _teamInfos[i].gameObject.SetActive(true);
                }
                else
                {
                    var newTeamInfo = Instantiate(_teamInfoPrefab, _container);
                    newTeamInfo.UpdateInfo(teamInfo.UnitTeam, teamInfo.WinsCount);
                    _teamInfos.Add(newTeamInfo);
                }
            }

            for (int i = teamsInfo.Count; i < _teamInfos.Count; i++)
            {
                _teamInfos[i].gameObject.SetActive(false);
            }
        }

        private void OnStartButtonClick()
        {
            OnStartLevelOpenRequest?.Invoke();
        }
    }
}