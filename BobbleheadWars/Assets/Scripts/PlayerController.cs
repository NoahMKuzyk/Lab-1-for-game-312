using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 50.0f;
    public Rigidbody head;
    private CharacterController characterController;
    public LayerMask layerMask;
    private Vector3 currentLookTarget = Vector3.zero;
    public Animator bodyAnimator;

    public float timeBetweenHits = 2.5f;
    private bool isHit = false;
    private float timeSinceHit = 0;
    private int hitNumber = -1;
    public float[] hitForce;

    public Rigidbody marineBody;
    private bool isDead = false;

    // Start is called before the first frame update
    void Start() {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isHit)
        {
            timeSinceHit += Time.deltaTime;
            if (timeSinceHit > timeBetweenHits)
            {
                isHit = false;
                timeSinceHit = 0;
            }
        }
        //Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

    }

    void FixedUpdate() {
        Vector3 pos = transform.position;
        Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        characterController.SimpleMove(moveDirection * moveSpeed);
        if (moveDirection == Vector3.zero)
        {
            bodyAnimator.SetBool("IsMoving", false);
            // TODO
        }
        else
        {
            bodyAnimator.SetBool("IsMoving", true);
            head.AddForce(transform.right * 150, ForceMode.Acceleration);
        }
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 1000, Color.green);
        if (Physics.Raycast(ray, out hit, 1000, layerMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.point != currentLookTarget)
            {
                currentLookTarget = hit.point;
            }
            Debug.Log("cock");
            // 1
            Vector3 targetPosition = new Vector3(hit.point.x,transform.position.y, hit.point.z);
            // 2
            Quaternion rotation = Quaternion.LookRotation(targetPosition - transform.position);
            // 3
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 10.0f);

        }
    }

    void OnTriggerEnter(Collider other)
    {
        Alien alien = other.gameObject.GetComponent<Alien>();
        if (alien != null)
        { // 1
            if (!isHit)
            {
                hitNumber += 1; // 2
                CameraShake cameraShake = Camera.main.GetComponent<CameraShake>();
                if (hitNumber < hitForce.Length) // 3
                {
                    cameraShake.intensity = hitForce[hitNumber];
                    cameraShake.Shake();
                }
                else
                {
                    // death todo
                }
                isHit = true; // 4
                SoundManager.Instance.PlayOneShot(SoundManager.Instance.hurt);
            }
            alien.Die();
        }
    }

}
