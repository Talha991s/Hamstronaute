/*  Author: Salick Talhah
 *  Date Created: February 6, 2021
 *  Last Updated: March 16, 2021
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    //Shooting Variables 
    public GameObject bullet;


    public Cinemachine.CinemachineVirtualCamera camController;
    public Transform lockOn; //lock cam position
    public Camera cam;

    // running speed
    public float speed;

    // Camera speed
    public float camSpeed = 20;
    public Vector3 camOffset;

    // jumping power
    public float jumpForce;

    // joystic object
    public Joystick moveStick;
    public Joystick camStick;

    public Rigidbody rb;
    public bool grounded;
    public float sensitivity;
    
    Vector3 moveJoyDir;

    float hor;
    float vert;
    public float turnSmoothVelocity;
    public float turnSmoothTime;
    public float angle;

    Animator anim;
    [SerializeField] bool isMoving;

    public enum ControlSettings
    {
        DESKTOP,
        MOBILE
    }

    public ControlSettings controlSettings;
    public bool inventory;
    
    /// Events
    void Start()
    {
        
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        InvertControls();
    }
    
    void Update()
    {
        lockOn.position = transform.position;

        if (controlSettings == ControlSettings.DESKTOP)
        {
            Controls();
        }
        else if (controlSettings == ControlSettings.MOBILE)
        {
            ControlsMobile();
        }

    }


    void Controls()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if (!inventory)
            {
                GameObject.FindGameObjectWithTag("Inventory").GetComponent<PlayerInventory>().OpenInvetory();
                inventory = true;
            }
            else
            {
                GameObject.FindGameObjectWithTag("Inventory").GetComponent<PlayerInventory>().CloseInvetory(true);
                inventory = false;
            }

        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

   

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        if (Input.GetKey(KeyCode.A))
        {
            angle -= 125 * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            angle += 125 * Time.deltaTime;
        }
        // Side to Side Movemnt
        if (Input.GetKey(KeyCode.A))
        {
            hor = -1;
        }
        else if(Input.GetKey(KeyCode.D))
        {
            hor = 1;
        }
        else
        {
            hor = 0;
        }

        // Forward and Back movement
        if(Input.GetKey(KeyCode.W))
        {
            vert = 1;
        }
        else if(Input.GetKey(KeyCode.S))
        {
            vert = -1;
        }
        else
        {
            vert = 0;
        }

        moveJoyDir = new Vector3(hor, 0, vert);

        float smoothedAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, angle, ref turnSmoothVelocity, turnSmoothTime);
        transform.rotation = Quaternion.Euler(0, smoothedAngle, 0);

        if (isMoving)
        {
            anim.SetInteger("AnimationPar", 1);
            

            Vector3 moveDir = gameObject.transform.forward * vert;
            transform.position += moveDir * speed * Time.deltaTime;
        }
        else
        {
            anim.SetInteger("AnimationPar", 0);
        }
    }

    void ControlsMobile()
    {
        moveJoyDir = new Vector3(moveStick.Horizontal, 0 ,moveStick.Vertical).normalized;


        if (moveJoyDir.magnitude >= sensitivity)
        {
            anim.SetInteger("AnimationPar", 1);
            float targetAngle = Mathf.Atan2(moveJoyDir.x, moveJoyDir.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
            angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);

            Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            transform.position += moveDir.normalized * speed * Time.deltaTime;
            
        }
        else
        {
            anim.SetInteger("AnimationPar", 0);
        }
            
        if(camStick.Horizontal >= sensitivity)
        {
            lockOn.transform.Rotate(0, camSpeed * Time.deltaTime, 0);
        }
        else if(camStick.Horizontal <= -sensitivity)
        {
            lockOn.transform.Rotate(0, -camSpeed * Time.deltaTime, 0);
        }

    }

    public void Shoot()
    {
        anim.SetTrigger("Shoot");
        Vector3 offset = transform.forward * 0.3f;
        GameObject b = Instantiate(bullet, transform.position + offset, new Quaternion(0, 0, 0, 0));
        b.GetComponent<LazerScript>().direction = transform.forward;
        FindObjectOfType<SoundManager>().Play("PlayerShoot");
    }

    public void Jump()
    {

        if (grounded && Time.timeScale != 0)
        {
            rb.AddForce(new Vector3(0, jumpForce, 0));
            FindObjectOfType<SoundManager>().Play("jump");
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        grounded = true;
    }
    private void OnCollisionExit(Collision other)
    {
        grounded = false;
    }

    // Added OnCollisionStay as I was getting inconsistent collision with imported assets
    // Also added Ground tag so you aren't able to jump along walls
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Platform"))
        {
            grounded = true;
        }
    }

    void InvertControls(){
        GameObject aBtn = GameObject.Find("A_Btn");
        GameObject bBtn = GameObject.Find("B_Btn");
        GameObject joystickLeft = GameObject.Find("Joystick Left");
        GameObject joystickRight = GameObject.Find("Joystick Right");
        Vector2 aPos = aBtn.transform.position;
        Vector2 bPos = bBtn.transform.position;
        Vector2 JL = joystickLeft.transform.position;
        Vector2 JR = joystickRight.transform.position;
        if(GameObject.FindGameObjectWithTag("SoundManager").GetComponent<ControlManager>().IsJumpInvert()){
            
           aBtn.transform.position = new Vector2(-aPos.x + Screen.width, aPos.y);
           bBtn.transform.position = new Vector2(-bPos.x + Screen.width, bPos.y);
        }
        if(GameObject.FindGameObjectWithTag("SoundManager").GetComponent<ControlManager>().IsMoveInvert()){
           joystickLeft.transform.position = JR;
           joystickRight.transform.position = JL;
        }
    }
}
