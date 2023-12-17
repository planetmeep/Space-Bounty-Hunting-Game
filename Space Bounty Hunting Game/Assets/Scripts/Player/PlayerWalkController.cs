using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkController : MonoBehaviour
{
    [SerializeField] private float walkSpeed;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform bodySpriteGroup;
    [SerializeField] private float bobAmplitude;
    [SerializeField] private float bobFrequency;
    private Rigidbody2D rb;
    private float timeMoved;
    Vector2 movementVelocity = Vector2.zero;
    Vector3 dampVelocity = Vector3.zero;
    Vector3 startPos;
    private bool walking;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        walking = false;
        startPos = bodySpriteGroup.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        movementVelocity = new Vector2(Input.GetAxis("Horizontal") * walkSpeed, Input.GetAxis("Vertical") * walkSpeed);
        walking = movementVelocity.magnitude > 0;

        bodySpriteGroup.localPosition += walking ? FootStepMotion(bobFrequency, bobAmplitude) : FootStepMotion(bobFrequency / 4, bobAmplitude / 6);
        ResetPosition();
    }
    private Vector3 FootStepMotion(float bobFrequency, float bobAmplitude)
    {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
        return pos;
    }

    private void ResetPosition()
    {
        if (bodySpriteGroup.localPosition == startPos) return;

        bodySpriteGroup.localPosition = Vector3.Lerp(bodySpriteGroup.localPosition, startPos, 1 * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        rb.velocity = movementVelocity;
    }
}
