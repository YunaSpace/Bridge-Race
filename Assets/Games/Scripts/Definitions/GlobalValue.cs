using Unity.Mathematics;

namespace YunaSpace.BridgeRace
{
    public class GlobalValue
    {
        public const string AnimationIdle = "Idle";
        public const string AnimationRun = "Run";
        public const string AnimationFall1 = "Fall 1";
        public const string AnimationFall2 = "Fall 2";
        public const string AnimationDance1 = "Dance 1";
        public const string AnimationDance2 = "Dance 2";
        public const string AnimationDance3 = "Dance 3";

        public static readonly int2 PlaygroundSize = new(12, 10);
        public const int MaxPlayerAmount = 6;
        public const int MaxBrickCarried = 30;
        public const float BrickStackSpace = 0.25f;

        public const float StateAvoidDistance = 2;
        public const float StateStumbleDuration = 1.5f;

        public const float StumbleForce = 5;

        public const float DropSpreadForce = 5;
        public const float DropBlowForce = 5;

        public const float GroundBrickSpace = 1.75f;
        public const float GroundBrickCooldown = 5f;

        public const int PlatformCurveResolution = 12;
        public const float PlatformHeight = 2f;
        public const float PlatformUVScale = 0.5f;
        public const float FenceThickness = 0.2f;
        public const float FenceHeight = 1.2f;
        public const float EntranceWidth = 3.8f;

        public const float DropBrickCollectableCooldown = 2f;

        public const float FlyingBrickThrowForce = 5f;
        public const float FlyingBrickLiftForce = 5f;
        public const float FlyingBrickCooldown = 1.5f;



        public static bool IsLevelPaused = false;
        public static bool IsLevelStarted = false;
    }
}