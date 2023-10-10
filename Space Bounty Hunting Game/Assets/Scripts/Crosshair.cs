using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public bool enableOnStart = true;
    SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        EnableCrosshair(enableOnStart);
    }

    public void UpdateCrosshairPosition(Vector2 mousePosition) 
    {
        /*Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePosition);
        transform.position = new Vector3(worldMousePos.x, worldMousePos.y, transform.position.z);*/
    }

    private void FixedUpdate()
    {
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(worldMousePos.x, worldMousePos.y, transform.position.z);
    }
    void EnableCrosshair(bool enable) 
    {
        spriteRenderer.enabled = enable;
        Cursor.visible = !enable;
    }
}
