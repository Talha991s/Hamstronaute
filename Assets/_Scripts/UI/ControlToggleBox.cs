/*  Author: Salick Talhah
 *  Date Created: March 27, 2021
 *  Last Updated: March 27, 2021
 *  Description: This script is used for the Option screen to toggle whether its inverted or not, connects option 
 *  buttons to sound manager/control manager (used this because it doesnt destroy on load)
 */
using UnityEngine;
using UnityEngine.UI;

public class ControlToggleBox : MonoBehaviour
{
    GameObject SoundManager;
    [SerializeField] GameObject invertJump, invertMove;
    bool gameStarted = false;
    // Start is called before the first frame update
    void Start()
    {
        SoundManager = GameObject.FindGameObjectWithTag("SoundManager");
        invertJump.GetComponent<Toggle>().isOn = SoundManager.GetComponent<ControlManager>().IsJumpInvert();
        invertMove.GetComponent<Toggle>().isOn = SoundManager.GetComponent<ControlManager>().IsMoveInvert();
        gameStarted = true;

    }

   public void ToggleInvertJump(){
       if(gameStarted){
        SoundManager.GetComponent<ControlManager>().ToggleInvertJump();
       }
    }
    public void ToggleInvertMove(){
         if(gameStarted){
          SoundManager.GetComponent<ControlManager>().ToggleInvertMove();
         }
    }
}
