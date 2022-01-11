/*  Author: Salick Talhah
 *  Date Created: March 1, 2021
 *  Last Updated: March 16, 2021
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Gold : MonoBehaviour
{
    public Quest quest;
    //private QuestGoal CollectedQuestGold;
    
    [SerializeField] private TMP_Text goldtext;

    [SerializeField] GameObject firstWinPlatform;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Goal"))
        {
            CollectGoal();
            Destroy(other.gameObject);
        }
    }

    public void CollectGoal()
    {
        if (quest.IsActive)
        {

            var goldCollected = quest.GoldenCoinCollected;
            goldCollected+=1;
            quest.GoldenCoinCollected += 1;
            goldtext.text = goldCollected.ToString();
            FindObjectOfType<SoundManager>().Play("collect");
            

            if (quest.goal.IsReached() || quest.GoldenCoinCollected ==10)
            {
                quest.Complete();
                Destroy(GameObject.FindWithTag("Door"));
                Destroy(firstWinPlatform);
            }
        }
    }

    public void SetGold(int _set) {
        quest.GoldenCoinCollected = _set;
        goldtext.text = quest.GoldenCoinCollected.ToString();
    }

}
