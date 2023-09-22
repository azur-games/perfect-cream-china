using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteriorUpgradeAnimation : MonoBehaviour
{
    public class MaterialDescription
    {
        private const string OVERLAPPED_COLOR_FIELD_NAME = "_OverlapedColor";
        private const string OVERLAPPED_POWER_FIELD_NAME = "_OverlapedPower";
        private static readonly Color OVERLAPPED_COLOR = Color.white;

        public Material Material { get; set; }

        public MaterialDescription(Material material)
        {
            Material = material;
            Material.SetColor(OVERLAPPED_COLOR_FIELD_NAME, OVERLAPPED_COLOR);
        }

        public void ApplyStepValue(float step)
        {
            Material.SetFloat(OVERLAPPED_POWER_FIELD_NAME, step);
        }
    }

    public class RendererDescription
    {
        public MeshRenderer MRenderer { get; set; }
        public List<MaterialDescription> Materials { get; set; }
        
        public void ApplyStepValue(float step)
        {
            foreach (MaterialDescription materialDescription in Materials)
            {
                materialDescription.ApplyStepValue(step);
            }
        }
    }

    public enum State
    {
        Creating,
        Staying,
        Destroying,
    }

    [SerializeField] private Vector3 pivot;
    [SerializeField] private AnimationCurve CreatingScaleCurve;
    [SerializeField] private AnimationCurve DestroyScaleCurve;

    private State state = State.Staying;
    private float timer = 0.0f;
    public float scale = 1.0f;
    private System.Action onFinish;
    private Dictionary<MeshRenderer, RendererDescription> renderers = null;
    private float creatingAnimationLength;
    private float un_creatingAnimationLength;
    private float destroyingAnimationLength;
    private float un_destroyingAnimationLength;

    private void Awake()
    {
        creatingAnimationLength = CreatingScaleCurve.keys[CreatingScaleCurve.keys.Length - 1].time;
        un_creatingAnimationLength = 1.0f / creatingAnimationLength;

        destroyingAnimationLength = DestroyScaleCurve.keys[DestroyScaleCurve.keys.Length - 1].time;
        un_destroyingAnimationLength = 1.0f / destroyingAnimationLength;
    }

    public void Play(State newState, System.Action onFinish)
    {
        state = newState;
        timer = 0.0f;
        
        this.onFinish = onFinish;

        if (state == State.Creating)
        {
            this.transform.localScale = new Vector3(1.0f, 0.000001f, 1.0f);
            this.transform.localPosition = new Vector3(0.0f, pivot.y, 0.0f);
        }
    }

    void Update()
    {
        float dTime = Time.deltaTime;
        switch (state)
        {
            case State.Staying:
                return;

            case State.Destroying:
                if (null == renderers) ReadMaterials();
                timer += dTime;
                scale = DestroyScaleCurve.Evaluate(timer);
                SetStepValues(Mathf.Clamp01(timer * un_destroyingAnimationLength));

                if (timer >= destroyingAnimationLength)
                {
                    state = State.Staying;
                    onFinish?.Invoke();
                }
                break;

            case State.Creating:
                if (null == renderers) ReadMaterials();
                timer += dTime;
                scale = CreatingScaleCurve.Evaluate(timer);
                SetStepValues(1.0f - Mathf.Clamp01(timer * un_creatingAnimationLength));

                if (timer >= creatingAnimationLength)
                {
                    state = State.Staying;
                    onFinish?.Invoke();
                }
                break;
        }

        this.transform.localScale = new Vector3(1.0f, scale, 1.0f);
        this.transform.localPosition = new Vector3(0.0f, pivot.y * (1.0f - scale), 0.0f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(pivot + this.transform.position, Vector3.one * 0.1f);
    }

    public void SetStepValues(float step)
    {
        step *= step;

        foreach (RendererDescription rendererDescription in renderers.Values)
        {
            rendererDescription.ApplyStepValue(step);
        }
    }

    private void ReadMaterials()
    {
        renderers = new Dictionary<MeshRenderer, RendererDescription>();
        foreach (MeshRenderer mRenderer in this.gameObject.GetComponentsInChildren<MeshRenderer>())
        {
            RendererDescription renDescription = new RendererDescription();
            renderers.Add(mRenderer, renDescription);

            renDescription.MRenderer = mRenderer;
            renDescription.Materials = new List<MaterialDescription>();
            foreach (Material material in mRenderer.materials)
            {
                renDescription.Materials.Add(new MaterialDescription(material));
            }
        }
    }
}
