using System.Collections.Generic;
using UnityEngine;

namespace YunaSpace.BridgeRace
{
    public class GoalPlatform : MonoBehaviour
    {
        [SerializeField] private StagePlatform platform;

        [SerializeField] private Transform[] models = new Transform[3];
        [SerializeField] private Renderer[] renderers = new Renderer[3];
        [SerializeField] private Animator[] animators = new Animator[3];
        [SerializeField] private List<ParticleSystem> particles = new();

        public void OnInitialize()
        {
            foreach (var model in models)
            {
                model.gameObject.SetActive(false);
            }

            foreach (var particle in particles)
            {
                particle.Stop();
            }
        }

        public void GeneratePlatform()
        {
            platform.GeneratePlatform();
        }

        public void ShowWinner(TeamRank rank)
        {
            for (int i = 0; i < 3; i++)
            {
                models[i].gameObject.SetActive(true);
                renderers[i].material.SetColor("_BaseColor", Game.ColorPalette.GetColorData(rank.Winners[i]).Color);
                animators[i].Play($"Dance {i + 1}");
            }

            foreach (var particle in particles)
            {
                particle.Play();
            }
        }
    }
}