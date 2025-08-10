using System;
using UnityEngine;

public class Enemy_02_Shoot : MonoBehaviour
{
    public Transform ShootingPoint;
    public GameObject DamageOrb;
    private Character cc;
    
    public AudioClip FireballCastClip;     
    private AudioSource _audioSource;     


    private void Awake()
    {
        cc=GetComponent<Character>();
        _audioSource = GetComponent<AudioSource>();
    }

    public void ShootTheDamageOrb()
    {
        if (FireballCastClip != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(FireballCastClip);
        }
        Instantiate(DamageOrb, ShootingPoint.position, Quaternion.LookRotation(ShootingPoint.forward));
    }

    private void Update()
    {
        cc.RotateToTarget();
    }
}
