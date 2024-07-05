using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Attributes")]
    [SerializeField] private float MoveSpeed = 5f;
    [SerializeField] private float EnemyAwarenessRadius = 2f;
    [SerializeField] private LayerMask SettlementLayer;
    [SerializeField] private LayerMask PlayerLayer;


    private Rigidbody2D Rigidbody;

    private Transform Target;
    private Settlement SettlementBeingGuarded;

    private Camera MainCamera;

    private Vector3 StartPos;
    // Start is called before the first frame update
    void Start()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        MainCamera = Camera.main;

        /*
        //Test code
        if (!SettlementBeingGuarded)
        {
            Target = GameObject.Find("Player").transform;
        }
        */

        //Target = GameObject.Find("Player").transform;
        StartPos = transform.position;
    }

    private void Update()
    {
        /*
        Collider2D SettlementOverlapped = Physics2D.OverlapCircle(transform.position, EnemyAwarenessRadius, SettlementLayer);
        if (SettlementOverlapped != null)
        {
            Target = SettlementOverlapped.transform;
        }
        */
        //logic for guarding settlement
        if (SettlementBeingGuarded)
        {
            Collider2D PlayerOverlapped = Physics2D.OverlapCircle(transform.position, EnemyAwarenessRadius, PlayerLayer);
            if (PlayerOverlapped != null)
            {
                print("player seen");
                Target = PlayerOverlapped.transform;
            }
            if (!PositionInCameraBounds(transform.position) && !PlayerOverlapped)
            {
                Target = null;
                //move back to start
                MoveTowardsTarget(StartPos);
            }
        }
        else
        {
            //when the settlement gets destroyed, go back to chasing the enemy
            if (!Target || !SettlementBeingGuarded)
            {
                Target = GameObject.Find("Player").transform;
            }
            //if there is a settlement
            Collider2D SettlementOverlapped = Physics2D.OverlapCircle(transform.position, EnemyAwarenessRadius, SettlementLayer);
            if (SettlementOverlapped != null)
            {
                Target = SettlementOverlapped.transform;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Target)
        {
            MoveTowardsTarget(Target.position);
        }
    }

    void MoveTowardsTarget(Vector3 TargetPosition)
    {
        //If close enough, stop moving
        if (Vector3.Distance(transform.position, TargetPosition) < 0.1f)  return; 
        Vector2 MoveDirection = (TargetPosition - transform.position).normalized;
        Rigidbody.MovePosition(Rigidbody.position + MoveDirection * MoveSpeed * Time.deltaTime);
    }

    public void GuardSettlement(Settlement SettlementToGuard)
    {
        //set the settlement here
        SettlementBeingGuarded = SettlementToGuard;
    }

    bool PositionInCameraBounds(Vector3 Position)
    {
        // Convert object's position to viewport space
        Vector3 ViewportPos = MainCamera.WorldToViewportPoint(Position);

        // Check if object is within camera bounds
        if (ViewportPos.x >= 0 && ViewportPos.x <= 1 &&
            ViewportPos.y >= 0 && ViewportPos.y <= 1 &&
            ViewportPos.z > 0) // Ensure the object is in front of the camera
        {
            return true;
        }
        return false;
    }
}
