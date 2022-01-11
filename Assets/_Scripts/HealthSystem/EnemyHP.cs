//Script By: Salick Talhah
//Edited By: 
//Date Created : Feburary 9th, 2021
//Date Edited : March 25th, 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    public GameObject poof;
    public int BaseHP = 10;
    public int hp;
    

    public float deathTime = 0.5f;

    void Start()
    {
        hp = BaseHP;
    }

    void Die()
    {
        Instantiate(poof,transform.position, transform.rotation);
        if(gameObject.name == "PA Drone"){
            FindObjectOfType<SoundManager>().Stop("LaserCharge");
        }
        Destroy(gameObject);
    }

    public void TakeDamage()
    {
        hp--;
        if (hp <= 0)
        {
            Die();
        }
        if(gameObject.name == "PA Warrior" || gameObject.name == "Spider"){
            int i = Random.Range(0,2);
            if(i == 0){
                FindObjectOfType<SoundManager>().Play("Enemy1Hit1");
            }else{
                FindObjectOfType<SoundManager>().Play("Enemy1Hit2");
            }
        }else if(gameObject.name == "Slime"){
            FindObjectOfType<SoundManager>().Play("SlimeHit");
        }else if(gameObject.name == "PA Drone"){
            FindObjectOfType<SoundManager>().Play("Enemy2Hit");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Bullet")
        {
            TakeDamage();
            //Debug.Log(gameObject.name + " HIT. HP is now " + hp);
            Destroy(other.gameObject);
        }
    }

}
