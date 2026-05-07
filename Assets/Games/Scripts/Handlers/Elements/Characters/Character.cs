using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

namespace YunaSpace.BridgeRace
{
    public class Character : MonoBehaviour
    {
        public bool IsPaused => isPaused;
        public bool IsStumbling => isStumbling;
        public int CurrentStage => currentStage;
        public Rigidbody Rigidbody => rigidbody;
        public NavMeshAgent Agent => agent;

        public int BrickCount => brickCount;
        public int TotalBrickCount => totalBrickCount;
        public bool IsBuilding => isBuilding;


        public bool IsDestination => Vector3.Distance(transform.position, destination + (transform.position.y - destination.y) * Vector3.up) < 0.1f;
        public bool IsOnGround => agent.isOnNavMesh;
        public float RemainingDistance => agent.remainingDistance;


        public ColorType ColorType;

        public CharacterBrickHandler BrickHandler;
        public CharacterBridgeHandler BridgeHandler;
        public CharacterShoveHandler ShoveHandler;


        [SerializeField] protected NavMeshAgent agent;
        [SerializeField] private Animator animator;
        [SerializeField] private new Renderer renderer;

        [SerializeField] protected TextMeshPro nameBadge;
        [SerializeField] protected ParticleSystem brickBurstVFX;

        [SerializeField] protected int currentStage;
        

        protected bool isPaused = false;
        protected bool isBuilding = false;

        protected new Rigidbody rigidbody;
        protected bool isStumbling = false;

        [SerializeField] protected int brickCount;
        [SerializeField] protected int totalBrickCount;

        private Vector3 agentVelocity;
        private float stumbleRemainTime;
        private float stumbleStartTime;

        private Vector3 destination;


        protected virtual void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();

            GlobalEvent.OnLevelPaused += OnPause;
            GlobalEvent.OnLevelResumed += OnResume;
            GlobalEvent.OnLevelInitialized += OnInitialize;
        }

        protected virtual void OnDestroy()
        {
            GlobalEvent.OnLevelPaused -= OnPause;
            GlobalEvent.OnLevelResumed -= OnResume;
            GlobalEvent.OnLevelInitialized -= OnInitialize;
        }

        public virtual void OnInitialize()
        {

        }

        public virtual void OnStart()
        {

        }

        protected virtual void OnPause()
        {
            isPaused = true;

            animator.speed = 0;
            agentVelocity = agent.velocity;

            SetMovable(false);

            stumbleRemainTime = Time.time - stumbleStartTime;
        }

        protected virtual void OnResume()
        {
            isPaused = false;

            if (IsOnGround)
            {
                SetMovable(true);

                agent.velocity = agentVelocity;
            }

            animator.speed = 1;

            if (isStumbling)
            {
                Invoke(nameof(ResetStumble), stumbleRemainTime);
            }
        }


        public void SetDestination(Vector3 destination)
        {
            this.destination = destination;
            agent.SetDestination(destination);
        }

        public void SetMovable(bool movable)
        {
            agent.isStopped = !movable;

            if (movable == false)
            {
                agent.velocity = Vector3.zero;
            }
        }

        public void SetSpeed(float speed)
        {
            agent.speed = speed;
        }

        public void Warp(Vector3 position)
        {
            agent.Warp(position);
        }


        public void SetColor(ColorType color)
        {
            ColorType = color;

            renderer.material = Game.ColorPalette.CharacterMaterial;
            renderer.material.SetColor("_BaseColor", Game.ColorPalette.GetColorData(ColorType).Color);

            var vfxMain = brickBurstVFX.main;

            vfxMain.startColor = Game.ColorPalette.GetColorData(color).Color;
        }

        public void AddBrick(bool toShow)
        {
            brickCount += toShow ? 1 : -1;

            if (toShow)
            {
                totalBrickCount++;

                brickBurstVFX.Play();
            }

            BrickHandler.ShowBrick(toShow);
        }

        public void PlayAnimation(string animation)
        {
            animator.WaitPlay(animation, 0.2f);
        }


        public virtual void OnFullBridgeReached(int nextStage)
        {
            currentStage = nextStage;

            if (currentStage < Game.LevelBuilderManager.MaxStage)
            {
                Game.StageGroundManager.ShowAllBrickOfColor(ColorType, currentStage);
            }
            else
            {
                GlobalEvent.OnLevelFinished?.Invoke();
            }
        }

        public virtual void OnBrickCountChanged()
        {

        }


        public void SetIsBuilding(bool isBuilding)
        {
            this.isBuilding = isBuilding;
        }

        public virtual void Stumble(Vector3 direction)
        {
            isStumbling = true;

            DropBrick();

            Invoke(nameof(ResetStumble), GlobalValue.StateStumbleDuration);

            BrickHandler.EnableCollider(false);

            stumbleStartTime = Time.time;
        }

        protected virtual void ResetStumble()
        {
            if (isPaused)
            {
                return;
            }

            isStumbling = false;

            BrickHandler.EnableCollider(true);
        }


        protected void DropBrick()
        {
            var dropTransforms = BrickHandler.GetDroppedBrickTransform(this is Player);

            Game.DropBrickManager.DropBrick(dropTransforms);

            totalBrickCount -= brickCount;

            brickCount = 0;
        }
    }
}