using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class projectile_bullet : MonoBehaviour
{
    public float speed;
    public short damage;
    public float lifeBulletTime;
    private float chanceTohit = 0.6f;
    public GameObject bloodSFX;
    
    
    // Update is called once per frame
    void FixedUpdate()
    {
        
        transform.Translate(Vector2.down * speed * Time.deltaTime);
        Destroy(gameObject,lifeBulletTime);
        
    }

    private void OnTriggerEnter2D(Collider2D colission2D)
    {
        float probability = Random.Range(0f, 1f);
        switch (colission2D.tag)
        {
            case "Enemy":
                if (probability <= chanceTohit)
                {
                    colission2D.GetComponent<BaseSoldier>().TakeDamage(damage);
                    Instantiate(bloodSFX, transform.position, colission2D.transform.rotation);
                    Destroy(gameObject);  
                }
                break;
            case "Friendly":
                if (probability <= chanceTohit)
                {
                    colission2D.GetComponent<BaseSoldier>().TakeDamage(damage);
                    Instantiate(bloodSFX, transform.position, colission2D.transform.rotation);
                    Destroy(gameObject);
                }

                break;
        }
    }
}
