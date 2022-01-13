/*  Author: Salick Talhah
 *  Date Created: January 31, 2021
 *  Last Updated: February 14, 2021
 *  Description: This script is used for the main UI control and scene Transition of the game.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        FindObjectOfType<SoundManager>().Play("click");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    public void PlayAgain()
    {
        FindObjectOfType<SoundManager>().Play("click");
        SceneManager.LoadScene(1);
    }
    public void BackToMainMenu()
    {
        FindObjectOfType<SoundManager>().Play("click");
        SceneManager.LoadScene(0);
    }
    public void OnLoadClick()
    {
        FindObjectOfType<SoundManager>().Play("click");
    }
    public void OnInstructionClick()
    {
        FindObjectOfType<SoundManager>().Play("click");
    }
}
