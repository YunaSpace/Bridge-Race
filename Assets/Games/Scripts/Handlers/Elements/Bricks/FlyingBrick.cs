using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

namespace YunaSpace.BridgeRace
{
    public class FlyingBrick : PoolUnit
    {
        public override float Cooldown => GlobalValue.FlyingBrickCooldown;

        [SerializeField] private new Renderer renderer;
        [SerializeField] private TrailRenderer trail;

        private float trailTime;

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

        public void SetColor(ColorType colorType)
        {
            var material = Game.ColorPalette.GetColorData(colorType).BrickMaterial;

            renderer.material = material;
            trail.material = material;
        }

        public void EnableTrail(bool enabled)
        {
            trail.Clear();
            trail.emitting = enabled;
        }

        private void OnPaused()
        {
            trailTime = trail.time;
            trail.emitting = false;
            trail.time = float.PositiveInfinity;
        }

        private void OnResumed()
        {
            trail.emitting = true;
            trail.time = trailTime;
        }
    }
}