using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceStationWall : MonoBehaviour, IHittable
{
    public GameObject smallRicochetParticle;
    public GameObject largeRicochetParticle;
    public int[] shipLayers;
    public int[] groundLayers;
    public HitsoundMaterials hitsoundMaterial;

    public void OnHit(Projectile projectile, Vector2 hitPoint, Quaternion hitDirection)
    {
        AudioManager.instance.PlayImpactSound(hitsoundMaterial);

        foreach (int layer in shipLayers) 
        {
            if (projectile.owner.layer == layer) 
            {
                Instantiate(largeRicochetParticle, hitPoint, Quaternion.Euler(0, 0, hitDirection.eulerAngles.z + 180));
            }
        }

        foreach (int layer in groundLayers)
        {
            if (projectile.owner.layer == layer)
            {
                Instantiate(smallRicochetParticle, hitPoint, Quaternion.Euler(0, 0, hitDirection.eulerAngles.z + 180));
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
