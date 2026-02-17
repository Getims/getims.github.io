using System;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Gameplay.GameFlow.Logic;
using Project.Scripts.UI.Common.Panels;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Project.Scripts.UI.Game.Top
{
    public class TopGamePanel : UIPanel
    {
        [SerializeField]
        private Button _settingsButton;

        [SerializeField]
        private TeamInfo _teamInfoPrefab;

        [SerializeField]
        private Transform _container;

        [Inject] private IGameInfoService _gameInfoService;

        private readonly List<TeamInfo> _teamInfoList = new();

        public event Action OnSettingsClick;

        protected void Start()
        {
            _settingsButton.onClick.AddListener(OnSettingsButtonClick);
            _gameInfoService.OnUnitsCountUpdate += OnUnitsCountUpdate;
        }

        protected override void OnDestroy()
        {
            _settingsButton.onClick.RemoveListener(OnSettingsButtonClick);
            _gameInfoService.OnUnitsCountUpdate -= OnUnitsCountUpdate;
        }

        private void OnUnitsCountUpdate()
        {
            var dictionary = _gameInfoService.UnitsDictionary;

            foreach (var teamInfo in dictionary)
            {
                var team = _teamInfoList.FirstOrDefault(x => x.UnitTeam == teamInfo.Key);
                if (team != null)
                    team.UpdateInfo(teamInfo.Value);
                else
                {
                    var newTeamInfo = Instantiate(_teamInfoPrefab, _container);
                    newTeamInfo.Setup(teamInfo.Key);
                    newTeamInfo.UpdateInfo(teamInfo.Value);
                    _teamInfoList.Add(newTeamInfo);
                }
            }
        }

        private void OnSettingsButtonClick() => OnSettingsClick?.Invoke();
    }
}