/*  Author: Salick Talhah
 *  Date Created: February 6, 2021
 *  Last Updated: March 16, 2021
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public GameObject bullet;
    Animator anim;
    
    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            anim.SetTrigger("Shoot");
            Vector3 offset = transform.forward * 0.3f;
            GameObject b = Instantiate(bullet, transform.position + offset, new Quaternion(0,0,0,0));
            b.GetComponent<LazerScript>().direction = transform.forward;
        }
    }
}
