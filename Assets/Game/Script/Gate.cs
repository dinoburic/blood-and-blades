using System;
using System.Collections;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public GameObject gateVisual;
    private Collider _gatecollider;
    public float openDuration = 2f;
    public float openTargetY = -2f;
    
    public AudioClip GateOpenSound;     
    private AudioSource _audioSource;  

    private void Awake()
    {
        _gatecollider = GetComponent<Collider>();
        _audioSource = GetComponent<AudioSource>();
    }

    IEnumerator OpenGateAnimation()
    {
        float currentOpenDuration = 0;
        Vector3 startPos = gateVisual.transform.position;
        Vector3 targetPos = startPos+Vector3.up*openTargetY;

        while (currentOpenDuration < openDuration)
        {
            currentOpenDuration += Time.deltaTime;
            gateVisual.transform.position = Vector3.Lerp(startPos, targetPos, currentOpenDuration / openDuration);
            yield return null;
        }

        _gatecollider.enabled = false;
    }

    public void Open()
    {
        if (GateOpenSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(GateOpenSound);
        }
        StartCoroutine(OpenGateAnimation());
        
    }
}
