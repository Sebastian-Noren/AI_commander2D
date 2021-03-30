using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BaseSoldier : MonoBehaviour
{
    [Serializable]
    public enum State
    {
        HoldPosition,
        Roaming,
        ChaseTarget,
        SearchAmmo,
    }

    // Start is called before the first frame update


    private Transform enemyTarget;
    private Animator animation;

    public int magazine;
    public int roundsPerMagazine;
    private int ammoCount;
    private int magazineCount;

    public int health;
    public float speed;
    public bool isAlive = true;
    public State state;
    public string tagTarget;

    public float stopDistance;
    private float attackTime = 1;
    public float timeBetweenEnemyAttack;
    private AudioSource shootingSound;
    public Transform firePoint;
    private Rigidbody2D rb;
    public GameObject muzzle_flash;
    public GameObject bulletPrefab;


    private Vector3 startingPosition;
    private Vector3 roamPosition;

    private Ai_move _aiMove;
    private Ai_sight fov;

    public float range;
    public bool drawLine;

    public bool test = false;
   public bool isUnderAttack;

    public void Awake()
    {
        _aiMove = GetComponent<Ai_move>();
        fov = gameObject.GetComponentInChildren<Ai_sight>();
        rb = GetComponent<Rigidbody2D>();
        animation = GetComponent<Animator>();
        shootingSound = GetComponent<AudioSource>();
    }

    public virtual void Start() // virtual and public to be able to overwrite
    {
        //    player = GameObject.FindGameObjectWithTag(tagTarget).transform;

        startingPosition = transform.position;
        roamPosition = GetRomingPosition();
        _aiMove.speed = speed;
        ammoCount = roundsPerMagazine;
        magazineCount = magazine;
        range++;

        //    InvokeRepeating("updateTarget",0f,1f);
    }


    public void Update()
    {
        
        if (isAlive)
        {
            if (isUnderAttack || fov.targetFound)
            {
                switchToCombat();
            }
            switch (state)
            {
                case State.HoldPosition:
                    
                    break;

                case State.Roaming:
                    Roamingstate();

                    break;
                case State.ChaseTarget:
                    attackState();
                    break;
                case State.SearchAmmo:
                    if (magazineCount == magazine)
                    {
                        state = State.Roaming;
                    }

                    if (test)
                    {
                        searchAfterAmmo();   
                    }
                    break;
            }
        }
    }

    private void LateUpdate()
    {
        if (drawLine && enemyTarget != null)
        {
            Debug.DrawLine(transform.position, enemyTarget.position, Color.blue);
        }
    }


    private void searchAfterAmmo()
    {
        print("Searhing ammo");
        GameObject[] ammoBox = GameObject.FindGameObjectsWithTag("Ammo");
        float shortestDistance = Mathf.Infinity;
        GameObject closestAmmoBox = null;
        foreach (var ammo in ammoBox)
        {
            float distanceToAmmoBox = Vector3.Distance(transform.position, ammo.transform.position);
            if (distanceToAmmoBox < shortestDistance)
            {
                shortestDistance = distanceToAmmoBox;
                closestAmmoBox = ammo;
            }

            if (closestAmmoBox != null)
            {
                Vector3 ammoPos = closestAmmoBox.transform.position;
                _aiMove.moveTo(ammoPos);
                test = false;
            }
        }
    }


    private void updateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(tagTarget);
        float shortestDistance = Mathf.Infinity;
        GameObject closestEnemy = null;
        foreach (var enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                closestEnemy = enemy;
            }

            if (closestEnemy != null && shortestDistance <= range)
            {
                enemyTarget = closestEnemy.transform;
            }
        }
    }


    private void attackState()
    {
        updateTarget();
        if (enemyTarget != null)
        {

            if (enemyTarget.CompareTag("Dead"))
            {
                state = State.Roaming;
                isUnderAttack = false;
                fov.targetFound = false;
            }

            lookDir();
            if (Vector3.Distance(transform.position, enemyTarget.position) > stopDistance)
            {
                animation.SetBool("isMoving", true);
                transform.position = Vector3.MoveTowards(transform.position, enemyTarget.position,
                    speed * Time.deltaTime);
            }
            else if (Time.time >= attackTime)
            {
                attackTime = Time.time + timeBetweenEnemyAttack;
                animation.SetBool("isMoving", false);
                EnemyRangedAttack();
            }
        }

    }

    private void switchToCombat()
    {
        state = State.ChaseTarget;
        _aiMove.StopAIMovement();
        _aiMove.moving = false;
    }

    private void Roamingstate()
    {
        if (fov.targetFound)
        {
            switchToCombat();
        }
        else
        {
            if (!_aiMove.moving)
            {
                if (_aiMove.AIcheckReachTarget(roamPosition))
                {
                    _aiMove.moving = true;
                    animation.SetBool("isMoving", true);
                    _aiMove.moveTo(roamPosition);
                }
                else
                {
                    roamPosition = GetRomingPosition();
                }
            }

            float reachedPositionDistance = 1.5f;
            if (Vector3.Distance(transform.position, roamPosition) <= reachedPositionDistance)
            {
                //   print("At distination");
                roamPosition = GetRomingPosition();
                _aiMove.moving = false;
                animation.SetBool("isMoving", false);
            }
        }
    }

    public void EnemyRangedAttack()
    {
        animation.SetTrigger("attack");
        shootingSound.Play();
        Instantiate(muzzle_flash, firePoint.position, transform.rotation);
        Instantiate(bulletPrefab, firePoint.position, transform.rotation);
        ammoCount--;
        if (ammoCount == 0)
        {
            if (magazineCount != 0)
            {
                magazineCount--;
                ammoCount = roundsPerMagazine;
            }
            else
            {
                test = true;
                state = State.SearchAmmo;
            }
        }
    }

    public void reload()
    {
        ammoCount = roundsPerMagazine;
        magazineCount = magazine;
    }


    // damage to creature
    public virtual void TakeDamage(int damage)
    {
        if (isAlive)
        {
            health -= damage;
            isUnderAttack = true;
            if (health <= 0)
            {
                isAlive = false;
                gameObject.tag = "Dead";
                _aiMove.StopAIMovement();
                foreach (Transform child in gameObject.transform)
                {
                    Destroy(child.gameObject);
                }

                animation.SetBool("isDead", true);
                gameObject.GetComponent<CircleCollider2D>().enabled = false;
                gameObject.GetComponent<SpriteRenderer>().sortingOrder--;
            }
        }
    }


    private Vector3 GetRomingPosition()
    {
        return startingPosition + GetRandomDirection() * Random.Range(4f, 8f);
    }


    // Random on the x, random on the y. Normalize vector and return a  random vectorvector
    private Vector3 GetRandomDirection()
    {
        return new Vector3(Random.Range(-1f, 1f), Random.Range(-1, 1f)).normalized;
    }

    private void lookDir()
    {
        // Function that returns angels between two vectors from X angel. Returns in radients convert to degrees
        Vector2 lookDir = enemyTarget.position - transform.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg + 90f;
        rb.rotation = angle;
    }
}