/*  Author: Salick Talhah
 *  Date Created: March 27, 2021
 *  Last Updated: March 27, 2021
 *  Description: This script is used as a control manager for the game to keep track within your session of whether you inverted your controls or not. 
 *  this script is placed on the same object as the sound manager because it is already not destroyed on load. 
 */
using UnityEngine;

public class ControlManager : MonoBehaviour
{
    public bool invertJump = false;
    public bool invertMove = false;

    public void ToggleInvertJump(){
        invertJump = !invertJump;
    }
    public void ToggleInvertMove(){
        invertMove = !invertMove;
        Debug.Log(invertMove);
    }
    public bool IsJumpInvert(){
        return invertJump;
    }
    public bool IsMoveInvert(){
        return invertMove;
    }
}
