using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayDeathSound : MonoBehaviour
{
    public AudioClip[] clips;
    AudioSource audioSource;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.clip = clips[0];
        print(audioSource.clip);
        audioSource.Play();
    }

}
