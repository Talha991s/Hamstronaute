/*  Author: Salick Talhah
 *  Date Created: February 6, 2021
 *  Last Updated: March 16, 2021
 *  Description: This script is a trigger for the seed to be collected
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedScript : MonoBehaviour
{
    private GameObject invScreen; //Reference to inventory gameobject/script
    public int seedWorth = 1; // how much is this seed worth.
    [SerializeField] string itemName;

    Vector3 startingPosition;
    bool isAcquirable = false;

    void Awake()
    {
        invScreen = GameObject.FindGameObjectWithTag("Inventory");
        itemName = gameObject.tag;
    }

    void Start()
    {
        startingPosition = transform.position;
        StartCoroutine(SeedAcquirableDelay()); //Delays the state when this seed becomes acquirable by the player.
    }
    void Update()
    {
        Hover();
        Rotate();
    }
    void Hover()
    {
        transform.position = Vector3.Lerp(startingPosition, startingPosition + new Vector3(0.0f, 0.1f, 0.0f), Mathf.PingPong(Time.time * 2, 1.0f));
    }

    void Rotate()
    {
        transform.Rotate(new Vector3(0.0f, 0.0f, 1.0f));
    }

    private void OnTriggerEnter(Collider col)
    {
        if (!isAcquirable) {
            return;
        }

        if (col.gameObject.tag == "Player")
        {
            invScreen.GetComponent<PlayerInventory>().CollectSeed(seedWorth);
            if (invScreen.GetComponent<PlayerInventory>().CollectItem(itemName))
            { //if item was collected destroy it
                Destroy(gameObject);
            }
        }
    }

    //Delays the state when this seed becomes acquirable by the player.
    //This is to prevent the player from acquiring seeds, that should be deleted, before they are deleted on saved game load.
    //Fixes the bug where player can acquire seeds that shouldnt exist on saved game load. 
    IEnumerator SeedAcquirableDelay() {
        isAcquirable = false;
        yield return new WaitForSeconds(0.4f);
        isAcquirable = true;
    }

}
