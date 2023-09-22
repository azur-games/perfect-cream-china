using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncedText : MonoBehaviour
{
    public List<UnityEngine.UI.Text> labels;

    public AnimationCurve animationCurve;

    private Vector3 defScale = Vector3.one;

    private float boucingTimer = 0.0f;

    private void Awake()
    {
        defScale = this.transform.localScale;
    }

    private void Update()
    {
        float scaleCoeff = animationCurve.Evaluate(boucingTimer);

        boucingTimer += Time.deltaTime;
        Vector3 scale = defScale;
        scale.y *= scaleCoeff;
        this.transform.localScale = scale;
    }

    public void SetWithBounce(string text)
    {
        this.gameObject.SetActive(true);

        foreach (UnityEngine.UI.Text label in labels)
        {
            label.text = text;
        }

        boucingTimer = 0.0f;
    }
}
