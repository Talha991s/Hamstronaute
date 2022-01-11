/*  Author: Talhah Salick
 *  Date Created: February 17, 2021
 *  Last Updated: February 17, 2021
 *  Description: This script is used item pick ups for the inventory.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupScript : MonoBehaviour
{
    [SerializeField] string itemName;
    private void OnTriggerEnter(Collider other)
    {
        GameObject inventory = GameObject.FindGameObjectWithTag("Inventory");
        if( inventory.GetComponent<PlayerInventory>().CollectItem(itemName)){ //if item was collected destroy it
            Destroy(gameObject);
        }
    }
}
