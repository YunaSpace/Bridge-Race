using System.Threading;
using UnityEngine;

namespace YunaSpace.BridgeRace
{
    public class DropBrick : PoolUnit
    {
        public override float Cooldown => 0;
        
        [SerializeField] private new Renderer renderer;
        [SerializeField] private new Rigidbody rigidbody;
        [SerializeField] private new Collider collider;
        [SerializeField] private Outline outline;

        private CancellationTokenSource cancellationTokenSource;

        private void Start()
        {
            renderer.material = Game.ColorPalette.GetColorData(ColorType.None).BrickMaterial;
        }

        private void OnDestroy()
        {
            OnDespawn();
        }

        public void OnInitialize()
        {
            collider.enabled = false;
            outline.enabled = false;
        }

        public override void OnSpawn()
        {
            cancellationTokenSource = new();
        }

        public override void OnDespawn()
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource?.Cancel();
                cancellationTokenSource?.Dispose();
                cancellationTokenSource = null;
            }
        }

        public void Drop()
        {
            rigidbody.isKinematic = false;
            rigidbody.linearVelocity = Vector3.zero;

            Vector2 randomCircle = Random.insideUnitCircle.normalized * GlobalValue.DropSpreadForce;
            Vector3 dropDirection = new Vector3(randomCircle.x, 0, randomCircle.y);

            rigidbody.AddForce(dropDirection, ForceMode.Impulse);
            rigidbody.AddForce(Vector3.up * GlobalValue.DropBlowForce, ForceMode.Impulse);

            EnableCollectable(cancellationTokenSource.Token);
        }

        private async void EnableCollectable(CancellationToken token)
        {
            try
            {
                float elapsed = 0;

                while (elapsed < GlobalValue.DropBrickCollectableCooldown)
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }

                    if (GlobalValue.IsLevelPaused == false)
                    {
                        elapsed += Time.deltaTime;
                    }

                    await Awaitable.NextFrameAsync();
                }

                collider.enabled = true;
                outline.enabled = true;
            }
            catch
            {

            }
        }
    }
}