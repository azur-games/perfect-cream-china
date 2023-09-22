using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CamShake : MonoBehaviour
{

    public float Amplitude = 0.3f;

    private float _duration = 0f;
    private float _currentAmplitude = 0;
    private float _damping;

    Vector3 originalPos;

    void Update()
    {
        if (_duration > 0)
        {
            _currentAmplitude -= Time.deltaTime * _damping;
            if (_currentAmplitude < 0)
                _currentAmplitude = 0;

            transform.localPosition = originalPos + Random.insideUnitSphere * _currentAmplitude;

            _duration -= Time.deltaTime;

            if (_duration <= 0)
                transform.localPosition = originalPos;
        }
    }

    public void Play(float duration)
    {
        if (_duration <= 0.0f)
        {
            originalPos = transform.localPosition;
        }

        _duration = duration;
        _currentAmplitude = Amplitude;
        _damping = Amplitude / _duration;
    }
}