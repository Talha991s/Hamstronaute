//Script By: Salick Talhah
//Edited By: 
//Date Created : Feburary 9th, 2021
//Date Edited : March 25th, 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    PlayerMovement player;

    enum PlatformType
    {
        BASIC,      // No movement
        MOVE,       // Simple movement
        ELEVATOR,   // Simple movement + brief pause at each point (currently unused)
        FLIP,       // Flips over after a pause
        FALLING     // Pauses, then falls after being landed on
    };

    [SerializeField] PlatformType platformType;

    /// Used for MOVE and ELEVATOR
    // positionOne is set to where it is placed in the world
    // positionTwoV is the vector derived from positionTwoT's transform, which is set and changed in Unity

    //Note:  made positional movement to be more similar to rotational movement because its easier to load on the savefile that way.
    //The prefabs now have a 'First Position' transform- separate from the actual platform.
    //Vector3 positionOne;
    //Vector3 positionTwoV;
    [SerializeField] Transform positionOneT; 
    [SerializeField] Transform positionTwoT;

    // Travel speed for the platform
    [SerializeField] float moveSpeed = 1.0f;

    /// Used for FLIP
    [SerializeField] Transform rotationOne;
    [SerializeField] Transform rotationTwo;

    [SerializeField] float rotateSpeed = 1.0f;
    [SerializeField] float flipStart = 0.0f;

    bool playerIsOnPlatform = false;

    bool hold = true;                   // First four currently unused, intended to control the pause for certain platforms
    float flipTimer = 2.5f;
    float holdPositionTimer = 5.0f;
    bool move = false;

    ///  Used for FALLING
    bool fall = false;          // Set to true when ready to fall
    bool shake = false;         // Shake "animation" to indicate fall incoming
    float fallSpeedIncrease;    // Makes the platform increase as it falls rather than moving down like an elevator


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindObjectOfType<PlayerMovement>();

        //Note:  made positional movement to be more similar to rotational movement because its easier to load on the savefile that way.

        //if (platformType == PlatformType.MOVE || platformType == PlatformType.ELEVATOR || platformType == PlatformType.FALLING)
        //{
        //    positionOne = transform.position;
        //    positionTwoV = positionTwoT.position;
        //}
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Rotate();
        Shake();
        Fall();
    }

    void Move()
    {
        if (platformType == PlatformType.MOVE || platformType == PlatformType.ELEVATOR)
        {
            // Interpolate position between both, PingPong continues so long as first value keeps going, Time.time is perfect for it
            gameObject.transform.position = Vector3.Lerp(positionOneT.position, positionTwoT.position, Mathf.PingPong(Time.time * moveSpeed, 1.0f));
        }
    }

    void Rotate()
    {
        if (platformType == PlatformType.FLIP)
        {
            transform.rotation = Quaternion.Lerp(rotationOne.rotation, rotationTwo.rotation, Mathf.PingPong(flipStart + Time.time * rotateSpeed, 1.0f));
        }
    }

    void Fall()
    {
        if (platformType == PlatformType.FALLING)
        {
            if (fall)
            {
                fallSpeedIncrease += 0.05f;
                transform.position = transform.position - new Vector3(0.0f, moveSpeed * Time.deltaTime * fallSpeedIncrease, 0.0f);
            }
        }
    }

    void Shake()
    {
        if (platformType == PlatformType.FALLING)
        {
            if (shake)
            {
                Vector3 shakeLeft = positionOneT.position - new Vector3(0.08f, 0.0f, 0.0f);
                Vector3 shakeRight = positionOneT.position + new Vector3(0.08f, 0.0f, 0.0f);
                transform.position = Vector3.Lerp(shakeLeft, shakeRight, Mathf.PingPong(Time.time * moveSpeed * 10, 1.0f));
            }
        }
    }

    IEnumerator ActivateFall()
    {
        if (platformType == PlatformType.FALLING)
        {
            yield return new WaitForSeconds(0.5f);
            shake = false;

            yield return new WaitForSeconds(0.5f);

            fall = true;

            yield return new WaitForSeconds(2.5f);

            fall = false;

            fallSpeedIncrease = 0.0f;
            transform.position = positionOneT.position;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (platformType != PlatformType.BASIC && platformType != PlatformType.FALLING)
            {
                collision.gameObject.transform.SetParent(gameObject.transform);
            }

            if (platformType == PlatformType.FALLING && !fall)
            {
                shake = true;
                StartCoroutine(ActivateFall());
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.SetParent(null);
        }
    }
}
