using System.Collections.Generic;
using UnityEngine;

namespace YunaSpace.BridgeRace
{
    public class GameEnvironmentManager : MonoBehaviour
    {
        [SerializeField] private Material waterMaterial;
        [SerializeField] private List<Animator> decorationAnimators = new();

        private float totalTimePaused = 0f;
        private float pauseStartTime;

        private void Awake()
        {
            GlobalEvent.OnLevelPaused += OnPaused;
            GlobalEvent.OnLevelResumed += OnResumed;
        }

        private void OnDestroy()
        {
            GlobalEvent.OnLevelPaused -= OnPaused;
            GlobalEvent.OnLevelResumed -= OnResumed;
        }

        private void Start()
        {
            foreach (var animator in decorationAnimators)
            {
                animator.enabled = true;
                animator.speed = 1;
            }
        }

        private void OnPaused()
        {
            pauseStartTime = Time.time;

            waterMaterial.SetInt("_IsPaused", 1);
            waterMaterial.SetFloat("_TimeStall", Time.time);

            foreach (var animator in decorationAnimators)
            {
                animator.enabled = false;
            }
        }

        private void OnResumed()
        {
            float currentPauseDuration = Time.time - pauseStartTime;

            totalTimePaused += currentPauseDuration;

            waterMaterial.SetInt("_IsPaused", 0);
            waterMaterial.SetFloat("_TimeOffset", totalTimePaused);

            foreach (var animator in decorationAnimators)
            {
                animator.enabled = true;
            }
        }
    }
}