using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    Image image;
    public bool enableOnStart = true;
    
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        EnableCrosshair(enableOnStart);
    }

    private void Update()
    {
        transform.position = Input.mousePosition;
    }
    void EnableCrosshair(bool enable) 
    {
        image.enabled = enable;
        Cursor.visible = !enable;
    }
}
