using Unity.Mathematics;
using UnityEngine;

namespace YunaSpace.BridgeRace
{
    public class CameraFocus : MonoBehaviour
    {
        [SerializeField] private new Transform camera;
        [SerializeField] private float3x3 cameraLimitation;
        [SerializeField] private float focusSpeed;

        private float3 currentOffset;

        private void Awake()
        {
            GlobalEvent.OnLevelLoaded += OnLevelLoaded;
            GlobalEvent.OnLevelFinished += OnLevelFinished;
        }

        private void OnDestroy()
        {
            GlobalEvent.OnLevelLoaded -= OnLevelLoaded;
            GlobalEvent.OnLevelFinished -= OnLevelFinished;
        }

        private void Update()
        {
            camera.localPosition = math.lerp(camera.localPosition, currentOffset, Time.deltaTime * focusSpeed);
        }

        public void UpdateFocus(int amount)
        {
            float t = 1.0f * amount / (GlobalValue.MaxBrickCarried);


            float3 focus = math.lerp(cameraLimitation.c0, cameraLimitation.c1, t);

            currentOffset = focus;
        }

        private void OnLevelLoaded(int level)
        {
            currentOffset = cameraLimitation.c0;
        }

        private void OnLevelFinished()
        {
            currentOffset = cameraLimitation.c2;
        }
    }
}
