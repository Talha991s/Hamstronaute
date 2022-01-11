/*  Author: Salick Talhah
 *  Date Created: February 14, 2021
 *  Last Updated: February 14, 2021
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningLaser : MonoBehaviour
{
    public float spinSpeed;
    
    void Update()
    {
        Spin();
    }

    void Spin()
    {
        transform.Rotate(new Vector3(0, 1, 0) * spinSpeed * Time.deltaTime);
    }
}
