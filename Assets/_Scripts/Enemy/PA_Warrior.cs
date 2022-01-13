//Script By: Salick Talhah
//Edited By: 
//Date Created : Feburary 9th, 2021
//Date Edited : March 25th, 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PA_Warrior : MonoBehaviour
{
    [SerializeField]


    NavMeshAgent NavAgent;
    public Transform player;
    public LayerMask GroundDetection, PlayerDetection; //Whats ground and whats player. 
    bool IsPointSet; // is the new walking position set?
    public Vector3 PointSet;  // New walking position to go to.
    public float ATKStall; //How long between attacks.
    bool ATKCooldown; //Has Already attacked, chill out!
    private Animator anim;
    public float PointRange; // How far new walking point can be. (Keep Reasonable)
    bool isATKStall;  // Has the enemy attacked yet if able to?, 
    public GameObject explosionPoof;

    public float AggroRange;  // Distance of aggro, detection.
    public float CombatDistance; // How far it can swing.
    public bool IsAggro, IsCombatDistance; // is in aggro range,  is it in combat range?

    [SerializeField] private bool IsDrone, isMoving; // is this a drone enemy // is the drone moving. // is the laser already cooled down // is the laser currently getting cooled down.

    private bool shooting, laserCD = false; //Is the enemy already shooting? // is the laser on cool down? /already fired
    [SerializeField] private Animator leftWing, rightWing; // is this a drone enemy // is the drone moving.

    private GameObject redDot;
    private Transform barrel; //reference to the barrel of the drone if it is a drone..

    [SerializeField] GameObject bullet;

    private Vector3 spawnPoint;

    // Start is called before the first frame update
    void Start()
    {

        anim = gameObject.GetComponent<Animator>();
    }
    private void Awake()
    {
        player = GameObject.Find("Stylized Astronaut").transform;
        NavAgent = GetComponent<NavMeshAgent>();
        spawnPoint = transform.position;
        if (IsDrone) {
            redDot = GameObject.Find("RedDot");
            barrel = transform.Find("Barrel");
        }

    }
    // Update is called once per frame
    void Update()
    {
        IsAggro = Physics.CheckSphere(transform.position, AggroRange, PlayerDetection);
        IsCombatDistance = Physics.CheckSphere(transform.position, CombatDistance, PlayerDetection);

        if (!IsDrone) {
            if (!IsAggro && !IsCombatDistance)
            {
                Patrol();
            }
            if (IsAggro && !IsCombatDistance)
            {
                Hunt();
            }
            if (IsCombatDistance && IsAggro)
            {

                AttackPlayer();
            }
            if (!IsCombatDistance)
            {
                anim.SetBool("Attack", false);
            }
        } else if (IsDrone) {
            DroneAI();
        }

    }
    private void ResetAttack()
    {

        ATKCooldown = false;

    }
    private void AttackPlayer()
    {
        NavAgent.SetDestination(transform.position);
        transform.LookAt(player);
        anim.SetBool("Attack", true);

        if (!isATKStall)
        {
            isATKStall = true;
            Invoke(nameof(ResetAttack), ATKStall);

        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            FindObjectOfType<SoundManager>().Play("Attacked");

        }


    }
    void OnTriggerEnter(UnityEngine.Collider collision)
    {
        if (collision.gameObject.CompareTag("Player") && (this.gameObject.CompareTag("Slime Enemy")))
        {
            
            Instantiate(explosionPoof, transform.position, transform.rotation);
            Destroy(gameObject);
            Destroy(this.gameObject,1);
            this.gameObject.SetActive(false);
        }
    }
    private void SearchWalkPoint()
    {
        
        float RandposX = Random.Range(-PointRange, PointRange);
        

        PointSet = new Vector3(transform.position.x + RandposX, transform.position.z);
        if (Physics.Raycast(PointSet, -transform.up, 2f, GroundDetection))
        {
            IsPointSet = true;

            anim.SetBool("Moving", true);
        }
    }

    private void Patrol()
    {
        if (!IsPointSet) SearchWalkPoint();
        if (IsPointSet)
        {
            NavAgent.SetDestination(PointSet);
            Vector3 distance = transform.position - PointSet;

            if (distance.magnitude < 1f)
                IsPointSet = false;
        }
    }

    private void Hunt()
    {
        NavAgent.SetDestination(player.position);
        anim.SetBool("Moving", true);
    }



//-----DRONE FUNCTIONS-----
    private void DroneAI(){
         if (!IsAggro && !IsCombatDistance)
        {
            FlyToStartPos();
        }
        if (IsAggro && !IsCombatDistance && !isMoving)
        {
            FlyAt();
        }
        if (IsAggro && !IsCombatDistance && isMoving && !shooting)
        {
            FlyTo();
        }
        if (IsCombatDistance && IsAggro)
        {
            ShootLaser();
        }
        if(shooting){
            ShootLaser();
        }
    }

    private void FlyToStartPos(){
        NavAgent.SetDestination(spawnPoint);
         if (!NavAgent.pathPending)
        {
            if (NavAgent.remainingDistance <= NavAgent.stoppingDistance)
            {
                if (!NavAgent.hasPath || NavAgent.velocity.sqrMagnitude == 0f)
                {
              
                     anim.SetBool("Moving", false);
                }
            }
        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("IdleAnim") )
        {
            //Cache in variable. <--

             //anim.SetBool("InAir", false);
             //   Transform[] ts = gameObject.transform.GetComponentsInChildren<Transform>();
             //   foreach (Transform t in ts){
             //        if (t.gameObject.name == "Particle"){
             //             t.GetChild(0).gameObject.SetActive(false);
             //        }
             //   }
             //   leftWing.enabled = false;
             //   rightWing.enabled = false;
             //   isMoving =false;
        }
        
    }
    private void FlyAt()
    {
        Transform[] ts = gameObject.transform.GetComponentsInChildren<Transform>();
         foreach (Transform t in ts){
             if (t.gameObject.name == "Particle"){
                t.GetChild(0).gameObject.SetActive(true);
             }
         }
         leftWing.enabled = true;
         rightWing.enabled = true;
         
        anim.SetBool("Moving", true);
       
        anim.SetBool("InAir", true);
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("MovingAnim"))
        {
            isMoving = true;
        }

    }

    private void FlyTo(){
       NavAgent.SetDestination(player.position);
    }

    private void ShootLaser(){
        NavAgent.SetDestination(transform.position); //stay in position
        transform.LookAt(new Vector3(player.position.x, player.position.y + 1, player.position.z)); //make enemy face player
        barrel.transform.LookAt(new Vector3(player.position.x, player.position.y - 0.2f, player.position.z));
        //check if its on cool down.
        if(!laserCD && !shooting){ //if laser isnt on cooldown.
            //start animation!
            shooting = true;
            anim.SetBool("Attack", true); //make attack animation play.
            FindObjectOfType<SoundManager>().Play("LaserCharge"); //Make laser charging sound.
            Debug.Log("Pew");
            redDot.SetActive(true); //show red dot in anim.
             
        }
        if(!laserCD && anim.GetCurrentAnimatorStateInfo(0).IsName("DoneShootingAnim")){ //Check if Shooting animation is done..?
                redDot.SetActive(false); //hide red dot in anim.
                GameObject b = Instantiate(bullet);
                b.transform.position = barrel.position;
                b.GetComponent<EnemyBullet>().direction = barrel.forward;
                FindObjectOfType<SoundManager>().Play("EnemyShoot"); //Make shooting sound.
                laserCD = true; //Laser is now on cool down till invoke goes off.
                Invoke(nameof(LaserRefresh), ATKStall);
                anim.SetBool("Attack", false); //make attack animation play.
        }
       
    }

    private void LaserRefresh(){
        //Laser ready to go again (already been shot)
        laserCD = false;
        shooting = false;
        
    }

    

 










}


