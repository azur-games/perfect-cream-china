using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Shape : ContentAsset
{
    public override AssetType GetAssetType() { return AssetType.Shape; }

    public bool HasSet;
    public float LengthSize;
    public bool IsFinalShape;

    public enum Type
    {
        None = 0,
        Shape = 0,
        Obstacle = 1,
        FinalObject = 2,
        FeverModeStartPlate = 3,
    }

    public class ByXPos : IComparer<Shape>
    {
        public int Compare(Shape a, Shape b)
        {
            return a.transform.position.x.CompareTo(b.transform.position.x);
        }
    }

    private static readonly int ShowTriggerHash = Animator.StringToHash("Show");

    [SerializeField] private Type selfType;

    public Collider[] Colliders;
    public Shape FinalShape;

    public float WidthHalf { get { return LengthSize * 0.5f; } }

    public bool HasAnimation { get { return _animator; } }
    [SerializeField] Animator _animator;
    public Transform CreamSpawnPoint;

    public bool IsAnimating { get { return _animTime > 0; } }
    public float ExtraTime { get; private set; }
    private float _animTime;
    private float _animTimer;
    int _activatedCount;

    float _scaleIncrement = 0.015f;
    
    private Animator topTextAnimator;
    private TMP_Text topText;



    private void Awake()
    {
        var roomConfig = Env.Instance.Rooms.GameplayRoom.Config as GameplayRoomConfig;
        var text = Instantiate(roomConfig.ShapeTopTextPrefab, transform);
        
        Colliders = GetComponentsInChildren<Collider>();
        
        topTextAnimator = text.GetComponent<Animator>();
        topText = text.GetComponent<TMP_Text>();
    }

    public void Animate()
    {
        _animator.gameObject.SetActive(true);
        AnimatorStateInfo info = _animator.GetCurrentAnimatorStateInfo(0);
        _animTime = info.length;
    }

    public Type GetSelfType() { return selfType; }

    private void Update()
    {
        if (!IsAnimating)
            return;

        _animTimer += Time.deltaTime;

        if(_activatedCount == Colliders.Length)
        {
            ExtraTime = _animTimer - _animTime;
            return;
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(this.transform.position + Vector3.left * LengthSize * 0.5f + Vector3.up * 0.5f, new Vector3(0.01f, 1.0f, 1.0f));
        Gizmos.DrawWireCube(this.transform.position - Vector3.left * LengthSize * 0.5f + Vector3.up * 0.5f, new Vector3(0.01f, 1.0f, 1.0f));
    }

    public void Move(Vector3 delta)
    {
        float positiveRange = Mathf.Max(Mathf.Min(transform.localPosition.x - 0.28f, delta.x), 0.0f);
        float negativeRange = delta.x - positiveRange;

        if (!IsFinalShape)
        {
            positiveRange += negativeRange;
            negativeRange = 0.0f;
        }

        // move self for positive range
        transform.localPosition -= delta;
    }

    public bool GetCollidersTopY(out float maxy)
    {
        maxy = float.MinValue;

        if (0 == Colliders.Length) return false;
        
        foreach (Collider collider in Colliders)
        {
            maxy = Mathf.Max(maxy, collider.bounds.max.y);
        }

        return true;
    }

    public void ShowText(string text)
    {
        topText.text = text;
        topTextAnimator.SetTrigger(ShowTriggerHash);
    }
}
