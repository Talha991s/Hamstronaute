//Script By: Salick Talhah
//Edited By: 
//Date Created : Feburary 9th, 2021
//Date Edited : March 25th, 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnLogic : MonoBehaviour
{
    //[SerializeField] GameObject currentSpawnPoint;
    //GameObject oldSpawnPoint;

    //Note: Used Vector3 instead of GameObject because it is easier to save/load from savefile
    [HideInInspector] public Vector3 currentSpawnPoint;
    [HideInInspector] public Vector3 currentSpawnPointRotation;
    [HideInInspector] public Vector3 oldSpawnPoint;

    [SerializeField] Transform cameraTransform;

    private void Awake() {
        currentSpawnPoint = this.transform.position;
        currentSpawnPointRotation = this.transform.localEulerAngles;
        oldSpawnPoint = currentSpawnPoint;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Hazard"))
        {
            FindObjectOfType<SoundManager>().Play("Dying");   
            //transform.position = currentSpawnPoint.transform.position;
            //transform.rotation = currentSpawnPoint.transform.rotation;
            transform.position = currentSpawnPoint;
            transform.eulerAngles = currentSpawnPointRotation;

            cameraTransform.transform.rotation = this.transform.rotation;
        }

        if (other.gameObject.CompareTag("Spawn"))
        {
            oldSpawnPoint = currentSpawnPoint;
            currentSpawnPoint = other.transform.position;
            currentSpawnPointRotation = other.transform.eulerAngles;
            //currentSpawnPoint = other.gameObject;
            //oldSpawnPoint = null;
        }
    }
}
