/*  Author: Salick Talhah
 *  Date Created: February 13, 2021
 *  Last Updated: February 13, 2021
 *  Description: This script is the Audio manager which make it simple to add a sound to a specific event. 
 *  
    Follow these steps to add the sound where you want: 
    
    Step 1 : Click on the sound manager and add the sound in the inspector.
    Step 2 : In the Inspector, Add the AudioMixer corresponding to the sound your are putting i.e sfx or music- (AudioMixer which is in the sound folder) 
    Step 3 : In the Inspector add the sound name and adjust the volume and other data.
    Step 4 : In you code, add this line to where you write the function of the event this line --->

      FindObjectOfType<SoundManager>().Play("name of the sound you added in the inspector");

    example: 

        private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Hazard"))
        {
  ----->     FindObjectOfType<SoundManager>().Play("Dying");           <----------------------------------------------
            transform.position = currentSpawnPoint.transform.position;
        }
    }
        
 */

using UnityEngine.Audio;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    //public AudioMixer audioMixer;
    public Sounds[] sounds;
   

    public static SoundManager instance;

    private void Awake()
    {
        // checking if we already have an audio in the scene.
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject); //keep the background music playing when cchanging scene
        foreach (Sounds s in sounds)
        {
            s.source=gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.mainmusic;
            s.source.outputAudioMixerGroup = s.sfxgroup;
        }



    }
    private void Start()
    {
        Play("Theme");
    }

    public void Play (string name)
    {
        Sounds s = Array.Find(sounds, sound => sound.name == name);
        if(s==null)
        {
            Debug.LogWarning("Sound: " + name + "not found! Check your sound name");
            return;
        }

        s.source.Play();
    }
    public void Stop (string name)
    {
        Sounds s = Array.Find(sounds, sound => sound.name == name);
        if(s==null)
        {
            Debug.LogWarning("Sound: " + name + "not found! Check your sound name");
            return;
        }

        s.source.Stop();
    }
}
