//Script By: Salick Talhah
//Edited By: 
//Date Created : Feburary 9th, 2021
//Date Edited : March 25th, 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyPlatform : MonoBehaviour
{
    // Strength of the bounce
    [SerializeField] float bounceStrength = 1.0f;

    // Possibility of activated or deactivated the platform, maybe for a puzzle?
    [SerializeField] bool isBouncy = true;


    void BouncePlayer(GameObject _player)
    {
        Vector3 force = new Vector3(0.0f, bounceStrength, 0.0f);
        _player.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isBouncy)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Debug.Log("Player");
                BouncePlayer(other.gameObject);
                FindObjectOfType<SoundManager>().Play("BouncePad");

            }
        }
    }
}
