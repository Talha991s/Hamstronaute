/*  Author: Salick Talhah; 
 *  Date Created: February 14, 2021
 *  Last Updated: March 16, 2021
 *  Description: This script is used to control the damage and amount of health, load the game over screen and check collision with hazard.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    //public Quest quest;

    [SerializeField] private GameObject gameover;
    [SerializeField] private GameObject win;

    public int maxhealth = 100;
    public int currentHealth = 100;
    public HealthBar healthBar;
    public TMP_Text winSceneText;       // Win scene text, will change depending on which exit you use 
    //private QuestGiver open;

    private bool isInvulnerable = true;
    public float DMGOverTime;     //How long IsCollisionStay needs to reach to increment a single damage tick. 
    public float IsCollisionStay;  //How Long The player is in the hitbox to be damaged. 


    // Start is called before the first frame update
    void Start()
    {
        if (currentHealth > maxhealth) 
        {
            currentHealth = maxhealth;
            healthBar.SetMaxHealth(maxhealth);
        }
        else if (currentHealth < 0) 
        {
            currentHealth = 0;
        }

        //Note: The lines below are commented out because it will interfere in the scenario where  will be loading a health amount from a save file.
        //currentHealth = maxhealth;
        //healthBar.SetMaxHealth(maxhealth);

        StartCoroutine(GameStartInvulnerability()); //Briefly disables character collision at game start.
    }

    // Update is called once per frame
    void Update()
    {
        
        if (currentHealth <= 0)
        {
           // FindObjectOfType<SoundManager>().Play("dead");
            GameOver();
            gameover.SetActive(true);
        }
    }
   public void TakeDamage(int damage)
   {
        if ( (currentHealth - damage) < 0) {
            currentHealth = 0;
        }
        else {
            currentHealth -= damage;
        }
        
        healthBar.SetHealth(currentHealth);
   }
    //--------------------QUEST-HAZARD-WIN-ENEMY-----------------//
    private void OnTriggerEnter(Collider other)
    {
        //if (other.gameObject.CompareTag("Goal"))
        //{
        //    FindObjectOfType<QuestGiver>().CollectGoal();
        //  //  Destroy();
        //}

        if (other.gameObject.CompareTag("QuestTrigger"))
        {
            FindObjectOfType<QuestGiver>().QuestWindowOpen();
        }

        if (other.gameObject.CompareTag("Hazard"))
        {
            TakeDamage(20);
        }
        if(other.gameObject.CompareTag("WinScene1"))
        {
            winSceneText.text = "You escaped, but couldn't save Polo...";
            Time.timeScale = 0.0f;
            win.SetActive(true);
        }
        if (other.gameObject.CompareTag("WinScene2"))
        {
            winSceneText.text = "You managed to escape and save Polo too!";
            Time.timeScale = 0.0f;
            win.SetActive(true);
        }
        if (other.gameObject.CompareTag("EnemyBullet"))
        {
            TakeDamage(10);
        }
        if (other.gameObject.CompareTag("Slime Enemy"))
        {
            FindObjectOfType<SoundManager>().Play("Attacked");
            GetComponent<Rigidbody>().AddForce(Vector3.back *200  + Vector3.up * 200);
            TakeDamage(15);
        }
    }
   void OnTriggerStay(UnityEngine.Collider other)
    {
        if (isInvulnerable) {
            return;
        }
        if (other.gameObject.CompareTag("Enemy")){
            IsCollisionStay += Time.deltaTime;
        }
        if (other.gameObject.CompareTag("Enemy") && IsCollisionStay >= DMGOverTime) 
        {
            
            IsCollisionStay = 0;
            FindObjectOfType<SoundManager>().Play("Attacked");
            TakeDamage(5);
        }
    }

    private IEnumerator GameOver()
    {
        yield return new WaitForSeconds(2.0f);
        //  gameover.SetActive(true);
    }

    
    public void AddHealth(int _amount)
    { //add amount of health from inventory screen when seed button is pressed
        if(currentHealth < maxhealth)
        {  //check if health is under max health
            currentHealth += _amount; //add amount to current health
            if(currentHealth > maxhealth)
            { //if health is greater then the max allowed set it to max allowed.
                currentHealth = maxhealth;
            }
        }
        healthBar.SetHealth(currentHealth);
    }
    
    //Note: Used in loading/saving game
    public void SetHealth(int _amount) 
    {
        if (_amount >= maxhealth) 
        {
            currentHealth = maxhealth;
            healthBar.SetMaxHealth(maxhealth);
        }
        else if (_amount < 0) 
        {
            currentHealth = 0;
        }
        else 
        {
            currentHealth = _amount;
        }
        healthBar.SetHealth(currentHealth);
    }

    //Briefly disables character collision at game start. This is to prevent errors when player decides to load a game where the player character is already colliding with an enemy.
    IEnumerator GameStartInvulnerability() {
        isInvulnerable = true;
        yield return new WaitForSeconds(0.4f);
        isInvulnerable = false;
    }


    //######################################################QUEST#########################################3


    
}
