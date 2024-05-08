using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    private static SoundPlayer instance;

    [SerializeField] private AudioSource deathAudio;
    [SerializeField] private AudioSource expAudio;
    [SerializeField] private AudioSource hurtAudio;

    float initialPitch;

    //PERMET AUX AUTRES ENTITÉS D'ALLER CHERCHER RÉFÉRENCE AU SOUNDPLAYER
    public static SoundPlayer GetInstance() => instance;

    private void Awake()
    {
        instance = this;
        initialPitch = deathAudio.pitch;
    }

    //JOUE LE SON DEATH DE BASE
    public void PlayDeathAudio()
    {
        deathAudio.pitch= initialPitch;
        deathAudio.Play();
    }

    //JOUE LE SON DEATH, PLUS AIGUE
    public void PlayHurtAudio()
    {
        deathAudio.pitch = 1.5f;
        deathAudio.Play();
    }

    public void PlayCollectingAudio() 
    {
        expAudio.Play();
    }

    public void PlayDamageAudio()
    {
        hurtAudio.pitch = 1f;
        hurtAudio.Play();
    }

    public void PlayFinalDamageAudio()
    {
        hurtAudio.pitch = 0.85f;
        hurtAudio.Play();
    }
}
