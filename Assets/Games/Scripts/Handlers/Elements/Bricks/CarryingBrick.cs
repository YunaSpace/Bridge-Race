using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;

namespace YunaSpace.BridgeRace
{
    public class CarryingBrick : PoolUnit
    {
        public override float Cooldown => 0;

        [SerializeField] private new Renderer renderer;

        private Material staticMaterial;
        private Material animatedMaterial;

        public void SetColor(ColorType colorType)
        {
            var colorData = Game.ColorPalette.GetColorData(colorType);

            staticMaterial = colorData.BrickMaterial;

            if (animatedMaterial == null)
            {
                animatedMaterial = new(staticMaterial);
            }

            animatedMaterial.SetColor("_BaseColor", colorData.Color);

            renderer.material = staticMaterial;
        }

        public void ShowAnimation()
        {
            animatedMaterial.SetFloat("_AnimationStartTime", Time.time);

            renderer.material = animatedMaterial;

            Invoke(nameof(Fallback), 1f);
        }

        private void Fallback()
        {
            renderer.material = staticMaterial;
        }
    }
}