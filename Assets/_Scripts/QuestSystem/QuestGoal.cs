/*  Author: Salick Talhah
 *  Date Created: March 1, 2021
 *  Last Updated: March 16, 2021
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestGoal 
{
    public GoalType goaltype;

    public int CollectedGold;
    

    public bool IsReached()
    {
        return (CollectedGold >= 10);
    }

}

public enum GoalType
{
    Kill,
    Gathering
}
