/*  Author: Salick Talhah
 *  Date Created: February 13, 2021
 *  Last Updated: February 13, 2021
 *  Description: This script holds a custom class to control what data is store in each sound.
 */

using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sounds 
{
    public AudioMixerGroup mainmusic;
    public AudioMixerGroup sfxgroup;

    public string name;

    public AudioClip clip;

    [Range(0.0f,1.0f)]
    public float volume;
    [Range(.1f,3.0f)]
    public float pitch;
    public bool loop;

    [HideInInspector]
    public AudioSource source;


}
