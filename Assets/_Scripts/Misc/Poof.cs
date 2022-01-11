/*  Author: Salick Talhah
 *  Date Created: February 6, 2021
 *  Last Updated: March 16, 2021
 */
using UnityEngine;
using UnityEngine.VFX;

public class Poof : MonoBehaviour
{

    VisualEffect self;
    bool started; 
    
    void Start()
    {
        self = GetComponent<VisualEffect>();
        started = false;
    }

    
    void Update()
    {
        if (self.aliveParticleCount == 32)
        {
            started = true;
        }

        if(self.aliveParticleCount == 0 && started)
        {
            Destroy(gameObject);
        }
    }
}
