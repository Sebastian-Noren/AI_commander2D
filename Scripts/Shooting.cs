
using UnityEngine;


public class Shooting : MonoBehaviour
{

    private Animator anim;
    public Transform firePoint;
    public GameObject bulletPrefab;
    public GameObject muzzle_flash;
    public float timeBetweenShoots;
    private float ShotTime;
    public AudioSource shootingSound;

    private void Start()
    {
        shootingSound = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
    }


    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();

        }
        
    }
    

    private void Shoot()
    {

        anim.SetTrigger("attack");
        shootingSound.Play();
        Instantiate(muzzle_flash, firePoint.position, transform.rotation);
        Instantiate(bulletPrefab, firePoint.position, transform.rotation);

    }

    public void die()
    {
        anim.SetBool("isDead", true);;
    }

}
