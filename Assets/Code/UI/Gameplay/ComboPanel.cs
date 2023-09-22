using DG.Tweening;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ComboPanel : MonoBehaviour
{
/*    [SerializeField] RectTransform _rt;
    [SerializeField] private Text _scoreFactor;
    
    [SerializeField] private float _resetSpeed = 0.1f;

    [SerializeField] private Image _progressFill;

    private float _currentScale = 1f;

    private Tweener _scaleTweener;

    private float _resetTime = 0.3f;
    private float _resetTimer = 0;
    private bool _isReseted = false;



    public void Charge(Score score)
    {
        _resetTimer = 0;

        if (_isReseted)
        {
            _currentScale = 1;
            _isReseted = false;
        }

        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        _scoreFactor.text = new StringBuilder("X").Append(score.Factor.ToString()).ToString();

        _progressFill.fillAmount = Mathf.Lerp(0.148f, 0.905f, (float)score.Factor / 11f);
    }

    public void Reset()
    {
        _isReseted = true;
    }


    private void Update()
    {
        if (_isReseted)
        {
        }
        else
        {
            _resetTimer += Time.deltaTime;
            if(_resetTimer > _resetTime)
            {
                Reset();
            }
        }

    }*/

}
