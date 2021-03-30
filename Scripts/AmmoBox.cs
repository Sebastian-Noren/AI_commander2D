using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBox : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Friendly") || other.CompareTag("Enemy"))
        {
            other.GetComponent<BaseSoldier>().reload();
        }
    }
    
}

