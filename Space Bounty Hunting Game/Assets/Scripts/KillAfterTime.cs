using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillAfterTime : MonoBehaviour
{
    [SerializeField] private float lifetime;
    private float timeAlive;

    // Start is called before the first frame update
    void Start()
    {
        timeAlive = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timeAlive += Time.deltaTime;
        if (timeAlive > lifetime) 
        {
            Destroy(gameObject);
        }
    }
}
