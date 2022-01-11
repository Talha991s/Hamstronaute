/*  Author: Salick Talhah
 *  Date Created: March 1, 2021
 *  Last Updated: March 16, 2021
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class QuestGiver : MonoBehaviour
{
    public Quest quest;
    public static bool IsQuestWindowOpen;
    public Gold player;
    //public GameObject player;
    public GameObject QuestScreen;
    public GameObject QuestTrigger;
    public GameObject goldCoins;

    [Header("Text")]
    public TMP_Text TitleText;
    public TMP_Text descriptionText;

    public void QuestWindowOpen()
    {
        IsQuestWindowOpen = true;
        QuestScreen.SetActive(true);
        TitleText.text = quest.title;
        descriptionText.text = quest.Description;
        FindObjectOfType<SoundManager>().Play("QuestMenu");
    }

    public void OnAcceptPressed()
    {
        goldCoins.SetActive(true);
        FindObjectOfType<SoundManager>().Play("QuestMenu");
        QuestScreen.SetActive(false);
        quest.IsActive = true;
        //quuuest.quest.IsActive = true;
        player.quest = quest;
    }

    //public void CollectGoal()
    //{
    //    if (quest.IsActive)
    //    {
    //        var goldCollected = CollectedQuestGold.CollectedGold;
    //        goldCollected++;
    //        goldtext.text = goldCollected.ToString();
    //        FindObjectOfType<SoundManager>().Play("collect");
    //       // Destroy(gameObject);

    //        if (goldCollected >=10)
    //        {
    //            quest.Complete();
    //            quest.IsActive = false;
    //            Debug.Log("Done");
    //        }
    //    }
    //}
}
