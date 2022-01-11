/*  Author:  Salick Talhah
 *  Date Created: February 3, 2021
 *  Last Updated: March 13, 2021
 *  Description: This script is used for managing everything related to the players inventory. 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInventory : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private PlayerHealth playerHealthRef;

    //Slider for inventory size, can be between 4 & 8 slots
    [Header ("Invetory Settings"), Range(4.0f, 8.0f)]
    [SerializeField] private int inventorySize = 8;
    
    private List<string> playerItems = new List<string>(); //players inventory items 
    [SerializeField] GameObject[] inventoryButtons; //Reference to the buttons in the inventory

    [SerializeField] Sprite[] inventorySprites; //Reference to whatever images you want for the items / inventory blank. 
    [SerializeField] Animator inventoryAnimator; //Reference to animator to change variable and make inventory trigger different anims
    private int inventorySlotTemp = 0;
    private bool itemExists = false;
    //----------------SEED VATRIABLES----------------------------
    private int playerSeeds = 0; //how many seeds the player has collected in the level
    [Header ("Seed Settings"), Space]
    [SerializeField] private Text playerSeedTxt; //reference to the player seed textbox
    private int totalSeeds = 0; //how many seed total in the entire level
    [SerializeField] private Text totalSeedTxt; //reference to the total seed textbox
    [SerializeField] private Animator seedTextAnimator; //reference to the animator to make seed count appear and disappear
    private bool showSeedText = false; //are you showing the seed count
    [SerializeField] private float seedShowTime = 3f; //How long to show the seed count for (seconds)
    private float scount = 0f; //Counter just to hold the amount of time elapsed since opened
    [SerializeField] int healAmount = 20; //Amount to heal the player when they eat the seed?

    //For Game Over Screen
    [Header("GameOver and Win Screen")]
    [SerializeField] private TMP_Text SeedCollected;
    [SerializeField] private TMP_Text OverTotalSeed;
    [SerializeField] private TMP_Text WinSeedCollected;
    [SerializeField] private TMP_Text WinTotalSeed;


    //---------------------- Unity Functions ------------------------------
    private void Awake(){
        //count how many seeds / total points the player has collected
        FindTotalSeeds();
        CloseInvetory(false);
    }
    void FixedUpdate(){
        //Counter to close players seeds collected when opened
        scount += Time.deltaTime;
        if(showSeedText){
            if(scount > seedShowTime){
                showSeedText = false;
                seedTextAnimator.SetBool("ShowCount", false);
            }
        }
        
    }

    //------------------- Inventory Functions ---------------------------------
    public void OpenInvetory() //Button calls this function to open the inventory / tab if on pc
    {
        FindObjectOfType<SoundManager>().Play("InventoryClick");
        gameObject.transform.SetAsLastSibling(); //Makes sure the inventory is in front of all other UI.
        inventoryAnimator.SetInteger("InventorySize", inventorySize); //changes inventory size in animator so it knows what inventory to animate.
        inventoryAnimator.SetBool("OpenInventory", true); //Change bool in animator to true so it opens
        seedTextAnimator.SetBool("ShowCount", true);
    }
    public void CloseInvetory(bool _closebutton) //Middle button in inventory calls this funtion to close the inventory / bool is whether it is a button click noise or not
    {
        if(_closebutton){
            FindObjectOfType<SoundManager>().Play("InventoryClick");
        }
        inventoryAnimator.SetBool("OpenInventory", false); //Change bool in animator to false so it closes
        seedTextAnimator.SetBool("ShowCount", false);
        showSeedText = false;
    }

    public bool CollectItem(string _name){ //Called when player enters trigger box of item. // items name is held within the item you are collectings script and is passed to this one.
    
        switch (_name){ //make sure the item string passed exists.
            case "Seed":  //For example if seed is the items string add it to the players item list
                itemExists = true;
                break;
            case "SuperSeed":  
                itemExists = true;
                break;
            default:
                Debug.Log("No string on item or non-existent"); //Error text
                return false;//tells item that it wasnt collected
            }
        if(itemExists){  //do this if the item above was exitent.
            if(playerItems.Count < inventorySize){ //check if theres room in inventory
                AddItemToList(_name);//add item to first empty spot in inventory.
                DisplayInventory(); //Display inventory / update images
                return true; //tells the item that it was collected.
            }
            else if(playerItems.Count >= inventorySize){ //if inventory is full, check to make sure none of the "full" spaces are actually empty spaces.
                for(int item = 0; item < playerItems.Count; item ++){ //cycle through inventory
                    if(playerItems[item] == null || playerItems[item] == ""){ //Check if item is empty or null so it can replace that space.
                        playerItems[item] = _name; //add item to that spot.
                        DisplayInventory(); //Display inventory / update images
                        return true; //tells item that it was collected
                    }
                }
            }
            Debug.Log("No room in inventory"); //Error text
            return true;//Tells that item was processed (even tho there wasnt room) // because they are like coins so they need to be collected either way.
          
        }
        
            Debug.Log("Non existent item"); //Error text
            return false;//Tells that item was not processed
    }
    public void AddItemToList(string _name){ //add item to inventory.
        for(int item = 0; item < playerItems.Count; item ++){
            if(playerItems[item] == null || playerItems[item] == ""){ //Check if item is empty or null so it can replace that space.
                playerItems[item] = _name;
                return;
            }
        }
            playerItems.Add(_name); //Add item to players item list
    }
    public void DisplayInventory(){ //Called when an inventory changes so it can update images.
         for(int i = 0; i < playerItems.Count; i++){ //Cycle through collected items
             switch(playerItems[i]){ //check strings of items
                 case "Seed":  //example if item name is seed display image on button
                    inventoryButtons[i].GetComponent<Image>().sprite = inventorySprites[1]; //Set inventory button image to seed sprite image
                    break;
                case "SuperSeed":  //example if item name is seed display image on button
                    inventoryButtons[i].GetComponent<Image>().sprite = inventorySprites[2]; //Set inventory button image to super seed sprite image
                    break;
                default:
                    inventoryButtons[i].GetComponent<Image>().sprite = inventorySprites[0]; //Set inventory button image to empty sprite image
                    Debug.Log("No picture for item set up"); //error text 
                    break;
             }
         }
    }
    public void InventoryButtonClick(Button button){ //Function triggered when the player clicks an inventory slot button
        string itemClicked = ""; //temp string to hold item thats in slot that was clicked

        if (!playerHealthRef) {
            Debug.LogError("Error! Could not find reference to player health.");
            return;
        }

        for(int i = 0; i < inventorySize; i++){ //cycle through all slots to check which button was clicked (in terms of 1-8)
            if(button.name == inventoryButtons[i].name){ //check if the button name that was passed when clicking button (in inspector), matches any of the buttons in the button array
                if(playerItems.Count > i){ //check if item even exists (only works because the empty spaces are counted as items).
                    itemClicked = playerItems[i]; //set temp string to hold item selected
                    playerItems[i] = "";
                    inventorySlotTemp = i;
                    Invoke("ClearInventorySlot", 0.8f); //wait 1 second before changing button back to nothing so it doesnt show in game
                }
                break;
            }
        }
        switch(itemClicked){ //Do whatever function that item would do
            case "Seed":  //for example if item was seed, it would then do its function here. 
                    Debug.Log("seed clicked");
                    playerHealthRef.AddHealth(healAmount);
                    //GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>().AddHealth(healAmount);
                    FindObjectOfType<SoundManager>().Play("EatSeed");
                    break;
            case "SuperSeed":  //Super seed gives double health as normal seed
                    Debug.Log("Super seed clicked");
                    playerHealthRef.AddHealth(healAmount * 2);
                    //GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>().AddHealth(healAmount * 2);
                    FindObjectOfType<SoundManager>().Play("EatSeed");
                    break; 
                default:
                    Debug.Log("No item in that slot"); //error message/tells user slot is empty
                    FindObjectOfType<SoundManager>().Play("InventoryClick");
                    break;
        }
        CloseInvetory(false); //Close inventory when an item slot is clicked
        
        

    }
    private void ClearInventorySlot(){
        inventoryButtons[inventorySlotTemp].GetComponent<Image>().sprite = inventorySprites[0]; //set button back to default image
    }
    /*
    - checks how many items in slots
    - checks if it already has that item
    - adds to first slot with string
    - clicking that item does a certain function. / some how get whatever button you press and get its name so you can match its name to an inventory slot or something. 
    */

    //Returns the string value of an inventory slot at a given index
    //Note: Used for saving inventory data to save file
    public string CheckItemAtInventorySlot(int _slotIndex) {
        if (_slotIndex >= playerItems.Count) {
            return "";
        }

        if (playerItems[_slotIndex] == null) {
            return "";
        }

        return playerItems[_slotIndex];
    }

    //----------------------SEED FUNCTIONS -----------------------
    public void CollectSeed(int _amount){
        //Gets called from "seed script" when player steps in a seeds trigger box, it gets sent that seeds value
        //that then is added to the players seed count, displayed, and resets counter to 0 for when the text disappears.
        playerSeeds += _amount;
        playerSeedTxt.text = playerSeeds.ToString();
        seedTextAnimator.SetBool("ShowCount", true);
        showSeedText = true;
        scount = 0f;
        FindObjectOfType<SoundManager>().Play("collect");  //added by salick
        SeedCollected.text = playerSeeds.ToString();
        WinSeedCollected.text = playerSeeds.ToString();
    }

    private void FindTotalSeeds(){
        //Find all seeds in level by finding all "seed" tagged objects and adding up their total worth.
        foreach(GameObject _seed in GameObject.FindGameObjectsWithTag("Seed")){
            totalSeeds += _seed.GetComponent<SeedScript>().seedWorth;
        }
        totalSeedTxt.text = totalSeeds.ToString();
        OverTotalSeed.text = totalSeeds.ToString();
        WinTotalSeed.text = totalSeeds.ToString();
    }

    //Returns player collected seed amount
    //Note: Used in loading/saving game
    public int GetPlayerSeedAmount() {
        return playerSeeds;
    }

    //Sets player collected seed amount
    //Note: Used in loading/saving game
    public void SetPlayerSeedAmount(int _amount) {
        playerSeeds = _amount;
        playerSeedTxt.text = playerSeeds.ToString();
        seedTextAnimator.SetBool("ShowCount", true);
        showSeedText = true;
    }
}

