/*  Author: Salick Talhah
 *  Date Created: February 6, 2021
 *  Last Updated: March 16, 2021
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerScript : MonoBehaviour
{
    public Vector3 direction;
    public float speed;

    Collider col;
    public MeshRenderer mesh;
    public TrailRenderer trail;
    public Color32 color;
    
    
    void Start()
    {
        col = GetComponent<Collider>();
        mesh = GetComponent<MeshRenderer>();
        trail = GetComponentInChildren<TrailRenderer>();
        trail.endColor = new Color32(0, 0, 0, 0);
        mesh.material.color = color;
        trail.startColor = color;
        transform.position += Vector3.up;
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player")
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag != "Player")
        {
            Destroy(gameObject);
        }
    }

}
