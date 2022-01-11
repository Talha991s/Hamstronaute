/*  Author: Salick Talhah
 *  Date Created: February 14, 2021
 *  Last Updated: February 14, 2021
 * Description: This script is used for updating the healthbar and change the color gradient as the player's life decreases.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;

      fill.color= gradient.Evaluate(1.0f);
    }

    public void SetHealth(int health)
    {
        slider.value = health;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
