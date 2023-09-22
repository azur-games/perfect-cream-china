using System.Collections.Generic;
using UnityEngine;

public class SceneVisual : MonoBehaviour
{
    private const int COLOR_SCHEMES_PERIOD = 3;
    [SerializeField] private  Renderer _rend;
    [SerializeField] private Camera _cam;

    public List<ColorScheme> Schemes;
    public ColorScheme FeverModeScheme;

    public int currentColorSchemeIndex = 0;

    private void Awake()
    {
        _rend.sharedMaterial = Material.Instantiate(_rend.sharedMaterial);
        UpdateBackSize();
    }

    private Vector3 moveOnUpdate = Vector3.zero;
    public void MoveBy(Vector3 moveBy)
    {
        moveOnUpdate += moveBy;
        LateUpdate();
    }

    private void LateUpdate()
    {
        this.transform.localPosition += moveOnUpdate;
        moveOnUpdate = Vector3.zero;
    }

    public void ApplyNextColorScheme()
    {
        currentColorSchemeIndex = (currentColorSchemeIndex + 1) % Schemes.Count;
        ApplyColorScheme(Schemes[currentColorSchemeIndex]);
    }

    public void ApplyRandomColorScheme()
    {
        currentColorSchemeIndex = Random.Range(0, Schemes.Count);
        ApplyColorScheme(Schemes[currentColorSchemeIndex]);
    }

    public void ApplyRandomColorScheme(List<ColorScheme> schemes)
    {
        if (schemes.Count < 1) return;

        ColorScheme scheme = schemes[Random.Range(0, schemes.Count)];
        ApplyColorScheme(scheme);
    }

    public void ApplyNextColorSchemeWithPeriod(int period = COLOR_SCHEMES_PERIOD)
    {
        int schemeIndex = PlayerPrefs.HasKey("schemeIndex") ? PlayerPrefs.GetInt("schemeIndex") : -1;
        schemeIndex++;
        PlayerPrefs.SetInt("schemeIndex", schemeIndex);

        currentColorSchemeIndex = (schemeIndex / period) % Schemes.Count;
        ApplyColorScheme(Schemes[currentColorSchemeIndex]);
    }

    public ColorScheme CurrentScheme { get; private set; }
    public void ApplyColorScheme(ColorScheme scheme)
    {
        CurrentScheme = scheme;
        SetBackgroundColor1(scheme.BackgroundBright);
        SetBackgroundColor2(scheme.BackgroundDark);
    }

    public static bool AllowToChangeBackgroundColors = true;

    private static Color backBrishtColor = Color.white;
    private Color color1Target = Color.white;
    private Color color1Current = Color.white;
    public void SetBackgroundColor1(Color color)
    {
        if (AllowToChangeBackgroundColors)
        {
            backBrishtColor = color;
        }
        else
        {
            color = backBrishtColor;
        }

        color1Target = color;
    }

    private static Color backDarkColor = Color.white;
    private Color color2Target = Color.white;
    private Color color2Current = Color.white;
    public void SetBackgroundColor2(Color color)
    {
        if (AllowToChangeBackgroundColors)
        {
            backDarkColor = color;
        }
        else
        {
            color = backDarkColor;
        }

        color2Target = color;
    }

    private UnityEngine.UI.Image thsImage = null;
    private void UpdateColors()
    {
        if (null == thsImage) thsImage = GameObject.FindObjectOfType<TopHeaderSprite>().gameObject.GetComponent<UnityEngine.UI.Image>();

        color1Current = Color.Lerp(color1Current, color1Target, 0.1f);
        color2Current = Color.Lerp(color2Current, color2Target, 0.1f);

        _rend.sharedMaterial.SetColor("_ColorGradientBright", color1Current);
        _rend.sharedMaterial.SetColor("_ColorGradientDark", color2Current);
        thsImage.color = color2Current;
    }

    private void Update()
    {
        UpdateColors();
    }

    public Color GetBackgroundColor1()
    {
        return _rend.sharedMaterial.GetColor("_ColorGradientBright");
    }

    public Color GetBackgroundColor2()
    {
        return _rend.sharedMaterial.GetColor("_ColorGradientDark");
    }

    public void UpdateBackSize()
    {
        float distance = _rend.transform.localPosition.z;

        float aspectRatio = Screen.width / (float)Screen.height;
        float height = 2.0f * Mathf.Tan(0.5f * _cam.fieldOfView) * distance;
        float width = height * aspectRatio;

        float scaleX = width / 2f ;
        float scaleY = height / 2f;

        _rend.transform.localScale = new Vector3(scaleX, scaleY, 1);
    }

}
