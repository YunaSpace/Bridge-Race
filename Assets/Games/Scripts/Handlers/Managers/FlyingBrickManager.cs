using System;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;

namespace YunaSpace.BridgeRace
{
    public class FlyingBrickManager : MonoBehaviour
    {
        [SerializeField] private PoolUnit brickPrefab;
        [SerializeField] private float flyDuration = 0.5f;

        private ColorPaletteSO colorPalette => GameManager.Instance.ColorPalette;

        private int poolID;

        private CancellationTokenSource _loopControlToken;

        private void Awake()
        {
            poolID = gameObject.GetInstanceID();

            ClearCancellationToken();

            GlobalEvent.OnLevelCleared += OnLevelCleared;
            GlobalEvent.OnLevelStarted += RestartCancellationToken;
        }

        private void Start()
        {
            PoolManager.Preload(poolID, brickPrefab, 5, transform);
        }

        private void OnDestroy()
        {
            ClearCancellationToken();

            PoolManager.Release(poolID);

            GlobalEvent.OnLevelCleared -= OnLevelCleared;
            GlobalEvent.OnLevelStarted -= RestartCancellationToken;
        }

        public void ShowFlying(ColorType color, Vector3 startPos, Transform target, System.Action onComplete)
        {
            var brick = PoolManager.Spawn<PoolUnit>(poolID, PoolType.FlyingBrick, startPos, Quaternion.identity) as FlyingBrick;
            brick.EnableTrail(true);
            brick.SetColor(color);

            try
            {
                FlyTask(brick, target, onComplete, _loopControlToken.Token);
            }
            catch (ObjectDisposedException) { } 
        }

        private void OnLevelCleared()
        {
            PoolManager.DespawnAll(poolID);

            ClearCancellationToken();
        }

        private void ClearCancellationToken()
        {
            try
            {
                _loopControlToken?.Cancel();
            }
            catch (ObjectDisposedException) { }
            
            _loopControlToken?.Dispose();
        }

        private void RestartCancellationToken()
        {
            _loopControlToken = new();
        }

        private async void FlyTask(PoolUnit brick, Transform target, System.Action onComplete, CancellationToken token)
        {
            var elapsed = 0f;
            Vector3 startPos = brick.Transform.position;
            Quaternion startRot = brick.Transform.rotation;

            Vector3 diff = (startPos - target.position).SetY(0);

            Vector3 dirAwayFromTarget;
            if (diff.sqrMagnitude < 0.001f)
            {
                dirAwayFromTarget = Vector3.forward;
            }
            else
            {
                dirAwayFromTarget = diff.normalized;
            }

            Vector3 controlPoint = startPos + (dirAwayFromTarget * GlobalValue.FlyingBrickThrowForce) + (Vector3.up * GlobalValue.FlyingBrickLiftForce);

            try
            {
                while (elapsed < flyDuration)
                {
                    if (token.IsCancellationRequested || brick == null || !brick.gameObject.activeInHierarchy || target == null)
                    {
                        return;
                    }

                    if (GlobalValue.IsLevelPaused)
                    {
                        await Awaitable.NextFrameAsync(token);
                        continue;
                    }

                    elapsed += Time.deltaTime;
                    float t = math.saturate(elapsed / flyDuration);

                    Vector3 currentTargetPos = target.position;

                    Vector3 bezierPos = math.pow(1 - t, 2) * startPos + 2 * (1 - t) * t * controlPoint + math.pow(t, 2) * currentTargetPos;

                    brick.Transform.position = bezierPos;
                    brick.Transform.rotation = Quaternion.Slerp(startRot, target.rotation, t);

                    await Awaitable.NextFrameAsync();
                }

                if (brick != null && target != null)
                {
                    brick.Transform.position = target.position;
                    brick.Transform.rotation = target.rotation;

                    onComplete?.Invoke();
                }
            }
            catch (System.OperationCanceledException)
            {

            }
            finally
            {
                if (brick != null)
                {
                    (brick as FlyingBrick).EnableTrail(false);
                    PoolManager.Despawn(brick, poolID);
                }
            }
        }
    }
}