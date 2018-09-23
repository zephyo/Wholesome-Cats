using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{

    public void Shake()
    {
        enabled = true;
    }

    // How long the object should shake for.
    private float shakeDuration = 0f;

    // Amplitude of the shake. A larger value shakes the camera harder.
    private float shakeAmount = 0.05f;
    private float decreaseFactor = 1.0f;


    void OnEnable()
    {
        shakeDuration = 0.18f;
    }

    void Update()
    {
        if (shakeDuration > 0)
        {
            transform.localPosition = Vector3.zero + Random.insideUnitSphere * shakeAmount;

            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            shakeDuration = 0f;
            transform.localPosition = Vector3.zero;
            enabled = false;
        }
    }
}

