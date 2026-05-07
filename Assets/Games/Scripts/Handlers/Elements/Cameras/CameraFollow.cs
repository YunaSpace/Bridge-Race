using System;
using Unity.Mathematics;
using UnityEngine;
using YunaSpace.BridgeRace;

public class CameraFollow : MonoBehaviour
{
    public Transform Target;
    public Vector3 Offset;
    public float Speed = 5f;

    private void Awake()
    {
        GlobalEvent.OnLevelLoaded += OnLevelLoaded;
        GlobalEvent.OnLevelFinished += OnLevelFinished;
        GlobalEvent.OnLevelMasked += OnLevelMasked;
    }

    private void OnDestroy()
    {
        GlobalEvent.OnLevelLoaded -= OnLevelLoaded;
        GlobalEvent.OnLevelFinished -= OnLevelFinished;
        GlobalEvent.OnLevelMasked -= OnLevelMasked;
    }

    private void LateUpdate()
    {
        Vector3 targetPos = Target.position + Offset;

        transform.position = Vector3.Lerp(transform.position, targetPos, 0.1f);
    }

    private void OnLevelLoaded(int level)
    {
        Target = Game.Player.transform;
    }

    private void OnLevelFinished()
    {
        Target = Game.LevelBuilderManager.GoalPlatform.transform;
    }

    private void OnLevelMasked()
    {
        transform.position = Game.Player.transform.position + Offset;
    }
}