using UnityEngine;

namespace YunaSpace.BridgeRace
{
    public class StairBrick : MonoBehaviour
    {
        public Bridge Bridge => bridge;

        public ColorType ColorType;
        public bool IsEnabled;

        [SerializeField] private new Renderer renderer;
        [SerializeField] private Bridge bridge;

        private Material staticMaterial;
        private Material animatedMaterial;

        public void SetBridge(Bridge bridge)
        {
            this.bridge = bridge;
        }

        public void UpdateStair(bool enabled, ColorType colorType = ColorType.None)
        {
            IsEnabled = enabled;

            renderer.gameObject.SetActive(enabled);

            if (enabled)
            {
                bridge.UpdateStairColorCount(this, ColorType, colorType);

                ColorType = colorType;

                var colorData = Game.ColorPalette.GetColorData(colorType);

                staticMaterial = colorData.StairMaterial;

                if (animatedMaterial == null)
                {
                    animatedMaterial = new(staticMaterial);
                }

                animatedMaterial.SetColor("_BaseColor", colorData.Color);
                animatedMaterial.SetFloat("_AnimationStartTime", Time.time);

                renderer.material = animatedMaterial;

                Invoke(nameof(Fallback), 1f);
            }

        }

        public bool CanPlaceStair(ColorType colorType)
        {
            return ColorType != colorType;
        }

        public bool IsFullBridge(ColorType colorType)
        {
            return bridge.IsFullBridge(colorType);
        }

        private void Fallback()
        {
            renderer.material = staticMaterial;
        }
    }
}