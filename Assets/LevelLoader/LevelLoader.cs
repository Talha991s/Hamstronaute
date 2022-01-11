/*  Author: Salick Talhah
 *  Date Created: March 1, 2021
 *  Last Updated: March 16, 2021
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator transtion;
    public float transitionTime = 2.5f;


    public void LoadNextLevel()
    {
        // add a coroutine to delay the transition
        StartCoroutine(Loadlevel(SceneManager.GetActiveScene().buildIndex +1));
        StartCoroutine(Loadlevel(SceneManager.GetActiveScene().buildIndex - 1));
    }

    IEnumerator Loadlevel(int levelindex)
    {
        //play animation
        transtion.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(levelindex);
    }
}
