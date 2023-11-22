using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bodyguard : MonoBehaviour, IKillable
{
    public SpriteRenderer[] handSprites;
    public GameObject gunPivot;
    public void Die()
    {
        foreach (var sprite in handSprites) 
        {
            sprite.color = Color.black;
        }
        gunPivot.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (var sprite in handSprites) 
        {
            sprite.color = Color.clear;
        }
        gunPivot.SetActive(true);
    }
}
