using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

namespace YunaSpace.BridgeRace
{
    public class CharacterTeamManager : MonoBehaviour
    {
        public List<ColorType> TeamColors => teamColors;
        public bool IsPlayerVictory => isPlayerVictory;

        [SerializeField] private Enemy enemyPrefab;
        [SerializeField] private Transform enemyContainer;

        [SerializeField] private float spawnSpace;
        [SerializeField] private Vector3 baseOffset;

        private List<float> spawnOffsets = new();
        private List<Character> teamCharacters = new();
        private List<ColorType> teamColors = new();

        private bool isPlayerVictory;

        private void Awake()
        {
            GlobalEvent.OnLevelLoaded += OnLevelLoaded;
            GlobalEvent.OnLevelStarted += OnMatchStarted;
        }
        
        private void OnDestroy()
        {
            GlobalEvent.OnLevelLoaded -= OnLevelLoaded;
            GlobalEvent.OnLevelStarted -= OnMatchStarted;
        }

        private void OnLevelLoaded(int level)
        {
            SelectParticipandColor();
            CreateParticipant();

            isPlayerVictory = false;
        }

        private void OnMatchStarted()
        {
            foreach (var character in teamCharacters)
            {
                character.OnStart();
            }
        }

        public TeamRank GetWinner()
        {
            var winners = teamCharacters.OrderByDescending(c => c.CurrentStage).ThenByDescending(c => c.TotalBrickCount).Select(i => i.ColorType).ToList();

            isPlayerVictory = winners[0] == Game.Player.ColorType;

            return new TeamRank(winners[0], winners[1], winners[2]);
        }

        public void SelectParticipandColor()
        {
            ClearParticipant();

            List<ColorType> allColors = Enum.GetValues(typeof(ColorType)).Cast<ColorType>().ToList();

            allColors.RemoveAll(c => c == ColorType.None);

            List<ColorType> shuffledColors = allColors.OrderBy(x => UnityEngine.Random.value).ToList();

            for (int i = 0; i < GlobalValue.MaxPlayerAmount; i++)
            {
                if (i < shuffledColors.Count)
                {
                    TeamColors.Add(shuffledColors[i]);
                }
            }
        }

        public void CreateParticipant()
        {
            spawnOffsets.Clear();
            teamCharacters.Clear();

            teamCharacters.Add(Game.Player);

            float totalWidth = (GlobalValue.MaxPlayerAmount - 1) * spawnSpace;
            float startX = -totalWidth / 2f;

            for (int i = 0; i < GlobalValue.MaxPlayerAmount; i++)
            {
                spawnOffsets.Add(startX + (i * spawnSpace));
            }

            spawnOffsets = spawnOffsets.OrderBy(x => UnityEngine.Random.value).ToList();

            var nameBadges = Game.NameBadge.GetRandomNames();

            for (int i = 0; i < GlobalValue.MaxPlayerAmount - 1; i++)
            {
                Vector3 spawnPos = new Vector3(spawnOffsets[i + 1], 0, 0);

                var enemy = Instantiate(enemyPrefab, enemyContainer);
                enemy.ChangeNameBadge(nameBadges[i]);
                enemy.transform.localPosition = spawnPos + baseOffset;

                if (i + 1 < TeamColors.Count)
                {
                    enemy.SetColor(TeamColors[i + 1]);
                }

                teamCharacters.Add(enemy);
            }

            Game.Player.SetColor(TeamColors[0]);
            Game.Player.Warp(new Vector3(spawnOffsets[0], 0, 0) + baseOffset);
        }

        public void HideParticipant()
        {
            ShowParticipant(false);
        }

        public void ShowParticipant(bool toShow = true)
        {
            foreach (var character in teamCharacters)
            {
                character.gameObject.SetActive(toShow);
            }

            Game.Player.gameObject.SetActive(toShow);
        }

        private void ClearParticipant()
        {
            TeamColors.Clear();

            enemyContainer.DestroyAllChildren();
        }
    }
}