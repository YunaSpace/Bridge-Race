using UnityEngine;

namespace YunaSpace.BridgeRace
{
    public class GroundBrick : MonoBehaviour
    {
        public ColorType ColorType => colorType;
        public Vector2Int Offset => offset;

        [SerializeField] private new Renderer renderer;
        [SerializeField] private Outline outline;

        private ColorType colorType;
        private Vector2Int offset;

        public void Initialize(int x, int y)
        {
            offset = new(x, y);
        }

        public void SetColor(ColorType colorType)
        {
            this.colorType = colorType;

            renderer.material = Game.ColorPalette.GetColorData(colorType).BrickMaterial;

            outline.enabled = colorType == Game.Player.ColorType;
        }
    }
}