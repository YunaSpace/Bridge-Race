using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace YunaSpace.BridgeRace
{
    public class CanvasMaskView : ViewCanvas<CanvasMaskView>
    {
        [SerializeField] private Image maskImage;
        [SerializeField] private float maskDuration;
        [SerializeField] private float maskStall;
        [SerializeField] private float2 maskSize;

        [SerializeField] private float timer;
        
        private Material maskMaterial;
        private bool isShowingMask;
        private bool isTransitioning;

        private void Awake()
        {
            maskMaterial = maskImage.material;
        }

        private void Update()
        {
            if (isTransitioning == false)
            {
                return;
            }

            if (isShowingMask && timer <= 0)
            {
                isTransitioning = false;

                GlobalEvent.OnLevelMasked?.Invoke();

                CloseMask();
                
                return;
            }

            if (!isShowingMask && timer >= maskDuration)
            {
                isTransitioning = false;

                this.gameObject.SetActive(false);

                return;
            }

            timer += isShowingMask ? -Time.deltaTime : Time.deltaTime;
            timer = math.clamp(timer, 0, maskDuration);

            var percent = timer / maskDuration;
            var size = math.remap(0, 1, math.max(maskSize.x, 0.0001f), maskSize.y, percent);
            maskMaterial.SetFloat("_MaskSize", size);
        }

        public override void OnOpened()
        {
            this.gameObject.SetActive(true);

            isShowingMask = true;
            isTransitioning = true;

            timer = maskDuration;
        }

        public override void OnClosed()
        {
            this.gameObject.SetActive(true);

            isShowingMask = false;
            isTransitioning = true;

            timer = 0;
        }

        private async void CloseMask()
        {
            try
            {
                await Awaitable.WaitForSecondsAsync(maskStall, destroyCancellationToken);

                OnClosed();
            }
            catch { }
        }
    }
}