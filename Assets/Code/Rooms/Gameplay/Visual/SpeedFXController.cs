using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static UnityEngine.ParticleSystem;

public class SpeedFXController : MonoBehaviour
{
    private const float ChangingSpeed = 0.1f;

    private class ParticlesState
    {
        private ParticleSystem ownParticleSystem;
        private ParticleSystemRenderer ownParticleSystemRenderer;
        private float startSpeed;
        private float rateOverTime;
        private float lengthScale;
        private MainModule main;
        private EmissionModule emission;

        public ParticlesState(ParticlesState copyFrom)
        {
            Init(
                copyFrom.ownParticleSystem, 
                copyFrom.ownParticleSystemRenderer, 
                copyFrom.startSpeed, 
                copyFrom.rateOverTime, 
                copyFrom.lengthScale);
        }

        public ParticlesState(
            ParticleSystem ownParticleSystem, 
            ParticleSystemRenderer ownParticleSystemRenderer, 
            float startSpeed, 
            float rateOverTime, 
            float lengthScale)
        {
            Init(ownParticleSystem, ownParticleSystemRenderer, startSpeed, rateOverTime, lengthScale);
        }

        private void Init(
            ParticleSystem ownParticleSystem,
            ParticleSystemRenderer ownParticleSystemRenderer,
            float startSpeed,
            float rateOverTime,
            float lengthScale)
        {
            this.ownParticleSystem = ownParticleSystem;
            this.ownParticleSystemRenderer = ownParticleSystemRenderer;
            this.startSpeed = startSpeed;
            this.rateOverTime = rateOverTime;
            this.lengthScale = lengthScale;

            main = ownParticleSystem.main;
            emission = ownParticleSystem.emission;
        }

        public void MoveTo(ParticlesState toState, float speed)
        {
            startSpeed = MoveFloat(startSpeed, toState.startSpeed, speed);
            rateOverTime = MoveFloat(rateOverTime, toState.rateOverTime, speed);
            lengthScale = MoveFloat(lengthScale, toState.lengthScale, speed);

            RefreshParticles();
        }

        private float MoveFloat(float from, float to, float speed)
        {
            return from * (1.0f - speed) + to * speed;
        }

        private void RefreshParticles()
        {
            main.startSpeed = startSpeed;
            emission.rateOverTime = rateOverTime;
            ownParticleSystemRenderer.lengthScale = lengthScale;
        }
    }

    [SerializeField] private Color baseColor;
    [SerializeField] private Color feverColor;

    [SerializeField] private ParticleSystem ownParticleSystem;
    [SerializeField] private ParticleSystemRenderer ownParticleSystemRenderer;
    private Dictionary<int, ParticlesState> stages;
    private ParticlesState currentState;
    private int currentStage;
    private int maxStage;

    private void CheckInited()
    {
        if (null != currentState) return;

        stages = new Dictionary<int, ParticlesState>();
        stages.Add(0, new ParticlesState(ownParticleSystem, ownParticleSystemRenderer, 0.0f, 0.0f, 0.0f));
        stages.Add(1, stages[0]);
        stages.Add(2, new ParticlesState(ownParticleSystem, ownParticleSystemRenderer, 5.0f, 20.0f, 40.0f));
        stages.Add(3, stages[2]);
        stages.Add(4, new ParticlesState(ownParticleSystem, ownParticleSystemRenderer, 20.0f, 60.0f, 80.0f));

        maxStage = (new List<int>(stages.Keys)).Max();

        currentStage = 0;
        currentState = new ParticlesState(stages[currentStage]);

        SetNormalColor();
    }

    private void Awake()
    {
        CheckInited();
    }

    public void SetNormalColor()
    {
        SetColor(baseColor);
    }

    public void SetFeverColor()
    {
        SetColor(feverColor);
    }

    private void SetColor(Color color)
    {
        MainModule particleSystemMain = ownParticleSystem.main;
        particleSystemMain.startColor = color;
    }

    public void SetMax()
    {
        currentStage = maxStage;
    }

    public void TryToIncrease()
    {
        currentStage = Mathf.Min(currentStage + 1, maxStage);
    }

    public void TryToDecrease()
    {
        currentStage = 0;
    }

    private void Update()
    {
        CheckInited();
        currentState.MoveTo(stages[currentStage], ChangingSpeed);
    }
}
