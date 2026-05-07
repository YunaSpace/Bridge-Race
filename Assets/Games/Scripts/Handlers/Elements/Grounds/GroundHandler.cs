using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace YunaSpace.BridgeRace
{
    public struct RespawnData
    {
        public ColorType Color;
        public float CollectTime;
    }

    public class GroundHandler : MonoBehaviour
    {
        [SerializeField] private GroundBrick brickPrefab;
        [SerializeField] private Transform brickContainer;

        private ColorPaletteSO colorPalette => GameManager.Instance.ColorPalette;
        private int width => GlobalValue.PlaygroundSize.x;
        private int height = GlobalValue.PlaygroundSize.y;

        private Dictionary<ColorType, int> colorCounts = new();
        private (ColorType? Type, GroundBrick Brick)[,] colorGrids;
        private Queue<RespawnData> respawnQueue = new();

        private System.Random randomer = new System.Random();

        private CancellationTokenSource _loopControlToken;

        private void Awake()
        {
            GlobalEvent.OnLevelCleared += ClearRespawnProcess;
            GlobalEvent.OnLevelStarted += StartRespawnProcess;
        }

        private void OnDestroy()
        {
            GlobalEvent.OnLevelCleared -= ClearRespawnProcess;
            GlobalEvent.OnLevelStarted -= StartRespawnProcess;
        }

        public void CollectBrick(GroundBrick brick)
        {
            colorGrids[brick.Offset.x, brick.Offset.y].Type = null;
            colorCounts[brick.ColorType]--;

            respawnQueue.Enqueue(new RespawnData
            {
                Color = brick.ColorType,
                CollectTime = Time.time
            });

            brick.gameObject.SetActive(false);
        }

        public GroundBrick FindNearestBrick(Vector3 worldPosition, ColorType targetColor)
        {
            var localPos = transform.InverseTransformPoint(worldPosition);

            var offsetX = (width - 1) * GlobalValue.GroundBrickSpace * 0.5f;
            var offsetZ = (height - 1) * GlobalValue.GroundBrickSpace * 0.5f;

            var startX = Mathf.RoundToInt((localPos.x + offsetX) / GlobalValue.GroundBrickSpace);
            var startY = Mathf.RoundToInt((localPos.z + offsetZ) / GlobalValue.GroundBrickSpace);

            startX = Mathf.Clamp(startX, 0, width - 1);
            startY = Mathf.Clamp(startY, 0, height - 1);

            var maxDistance = Mathf.Max(width, height);

            for (int radius = 0; radius < maxDistance; radius++)
            {
                for (int dy = -radius; dy <= radius; dy++)
                {
                    for (int dx = -radius; dx <= radius; dx++)
                    {
                        if (Mathf.Abs(dx) != radius && Mathf.Abs(dy) != radius) continue;

                        var nx = startX + dx;
                        var ny = startY + dy;

                        if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                        {
                            var cell = colorGrids[nx, ny];
                            if (cell.Type == targetColor && cell.Brick != null && cell.Brick.gameObject.activeInHierarchy)
                            {
                                return cell.Brick;
                            }
                        }
                    }
                }
            }

            return null;
        }

        public void ShowAllBrickOfColor(ColorType colorType)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    (ColorType? Type, GroundBrick Brick) colorCell = colorGrids[x, y];

                    if (colorCell.Type != ColorType.None && colorCell.Type == colorType)
                    {
                        colorCell.Brick.gameObject.SetActive(true);
                    }
                }
            }
        }

        public void CreateGroundBrick(bool toShow)
        {
            foreach (ColorType type in Game.CharacterTeamManager.TeamColors)
            {
                colorCounts[type] = 0;
            }

            colorGrids = new (ColorType?, GroundBrick)[width, height];

            int totalCells = width * height;
            int eachCount = totalCells / GlobalValue.MaxPlayerAmount;

            var colorPool = new List<ColorType>(totalCells);

            for (int i = 0; i < GlobalValue.MaxPlayerAmount; i++)
            {
                var type = Game.CharacterTeamManager.TeamColors[i];

                for (int j = 0; j < eachCount; j++)
                {
                    colorPool.Add(type);
                }
            }

            for (int i = 0; i < colorPool.Count; i++)
            {
                var rand = Random.Range(i, colorPool.Count);

                (colorPool[i], colorPool[rand]) = (colorPool[rand], colorPool[i]);
            }

            var index = 0;

            var offsetX = (width - 1) * GlobalValue.GroundBrickSpace * 0.5f;
            var offsetZ = (height - 1) * GlobalValue.GroundBrickSpace * 0.5f;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var localPos = new Vector3(x * GlobalValue.GroundBrickSpace - offsetX, 0, y * GlobalValue.GroundBrickSpace - offsetZ);

                    var brick = Instantiate(brickPrefab, Vector3.zero, Quaternion.identity, brickContainer.transform);
                    brick.Initialize(x, y);
                    brick.transform.localPosition = localPos;
                    brick.gameObject.SetActive(toShow);

                    var type = colorPool[index++];
                    brick.SetColor(type);

                    colorGrids[x, y] = (type, brick);
                    colorCounts[type]++;
                }
            }
        }

        private void ClearRespawnProcess()
        {
            _loopControlToken?.Cancel();
            _loopControlToken?.Dispose();
        }

        private void StartRespawnProcess()
        {
            _loopControlToken = new();

            var token = CancellationTokenSource.CreateLinkedTokenSource(_loopControlToken.Token, this.destroyCancellationToken).Token;

            RespawnLoop(token);
        }

        private async void RespawnLoop(CancellationToken token)
        {
            try
            {
                while (this != null && token.IsCancellationRequested == false)
                {
                    if (GlobalValue.IsLevelPaused)
                    {
                        await Awaitable.NextFrameAsync(token);
                        continue;
                    }

                    if (respawnQueue.Count > 0)
                    {
                        var data = respawnQueue.Peek();
                        float currentTime = Time.time;
                        float elapsed = currentTime - data.CollectTime;

                        if (elapsed < GlobalValue.GroundBrickCooldown)
                        {
                            await Awaitable.WaitForSecondsAsync(GlobalValue.GroundBrickCooldown - elapsed, token);
                        }

                        await Awaitable.BackgroundThreadAsync();

                        int startX = randomer.Next(0, width);
                        int startY = randomer.Next(0, height);

                        bool found = TryFindRightNearestEmpty(startX, startY, out int x, out int y);

                        await Awaitable.MainThreadAsync();

                        if (found)
                        {
                            RespawnBrick(x, y, data.Color);
                            respawnQueue.Dequeue();
                        }
                    }

                    await Awaitable.NextFrameAsync(token);
                }
            }
            catch (System.OperationCanceledException)
            {

            }
        }

        private bool TryFindRightNearestEmpty(int startX, int startY, out int x, out int y)
        {
            for (int dx = 0; dx < width; dx++)
            {
                int nx = (startX + dx) % width;

                if (colorGrids[nx, startY].Type == null)
                {
                    x = nx;
                    y = startY;
                    return true;
                }
            }

            for (int dy = 1; dy < height; dy++)
            {
                int nyUp = (startY + dy) % height;
                int nyDown = (startY - dy + height) % height;

                for (int dx = 0; dx < width; dx++)
                {
                    int nx = (startX + dx) % width;

                    if (colorGrids[nx, nyUp].Type == null)
                    {
                        x = nx;
                        y = nyUp;
                        return true;
                    }

                    if (colorGrids[nx, nyDown].Type == null)
                    {
                        x = nx;
                        y = nyDown;
                        return true;
                    }
                }
            }

            x = y = -1;
            return false;
        }

        private void RespawnBrick(int x, int y, ColorType color)
        {
            var brick = colorGrids[x, y].Brick;
            brick.SetColor(color);
            brick.gameObject.SetActive(true);

            colorGrids[x, y].Type = color;
            colorCounts[color]++;
        }
    }
}