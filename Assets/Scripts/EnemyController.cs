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

    private Vector3 SettlementGuardPos;

    private bool PlayerInRange = false;
    private bool AttackingSettlement = false;

    private bool PartOfEnemyCamp = false;
    // Start is called before the first frame update
    void Start()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        MainCamera = Camera.main;
        Target = GameObject.Find("Player").transform; //set the target initially 
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
        CheckForPlayer();
        CheckForSettlement();


    }


    private void CheckForSettlement()
    {
        if (!PlayerInRange || !PositionInCameraBounds(transform.position))
        {
            print("checking for settlement");
            //If there is a settlement, and we aren't already guarding a settlement
            Collider2D SettlementOverlapped = Physics2D.OverlapCircle(transform.position, EnemyAwarenessRadius, SettlementLayer);
            if (SettlementOverlapped != null && !SettlementBeingGuarded)//Target && Target.gameObject.layer != SettlementLayer)
            {
                //Can guard settlement if part of an enemy camp and it is uncaptured, or otherwise can only go after captured settlements
                Settlement SettlementToGuard = SettlementOverlapped.GetComponent<Settlement>();
                print(SettlementToGuard.IsCaptured);
                if (!SettlementToGuard.IsCaptured && PartOfEnemyCamp || SettlementToGuard.IsCaptured)
                {
                    GuardSettlement(SettlementOverlapped.GetComponent<Settlement>());

                }


            }
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Target)
        {
            //If moving towards settlement currently, then check for stuff
            if(SettlementBeingGuarded && Target == SettlementBeingGuarded.transform)
            {
                if(MoveTowardsTarget(Target.position, 2) && !AttackingSettlement)
                {
                    AttackSettlement();
                }
            }
            else
            {
                MoveTowardsTarget(Target.position);
            }
        }
    }

    private void AttackSettlement()
    {
        print("attack settlement");
        print(SettlementBeingGuarded);
        SettlementBeingGuarded.AddEnemyAttacker(); //close enough to settlement, so add attacker
        AttackingSettlement = true;
    }

    void CheckForPlayer()
    {
        Collider2D PlayerOverlapped = Physics2D.OverlapCircle(transform.position, EnemyAwarenessRadius, PlayerLayer);
        if (PlayerOverlapped != null)
        {
            PlayerInRange = true;
            print("player seen");
            Target = PlayerOverlapped.transform;
            StopAttackingSettlement();
        }
        else
        {
            PlayerInRange = false;//no player in range, but continue to chase if nthing else is in range
            if (SettlementBeingGuarded)
            {
                //go back to settement
                Target = SettlementBeingGuarded.transform;
            }
        }
    }

    private void StopAttackingSettlement()
    {
        if (SettlementBeingGuarded && AttackingSettlement)
        {
            //Remove attacker for now
            SettlementBeingGuarded.RemoveEnemyAttacker();
            AttackingSettlement = false;
        }
    }

    bool MoveTowardsTarget(Vector3 TargetPosition, float AcceptanceDistance = 0.1f)
    {
        //If close enough, stop moving
        if (Vector3.Distance(transform.position, TargetPosition) < AcceptanceDistance)  return true; 
        Vector2 MoveDirection = (TargetPosition - transform.position).normalized;
        Rigidbody.MovePosition(Rigidbody.position + MoveDirection * MoveSpeed * Time.deltaTime);

        return false;
    }

    public void GuardSettlement(Settlement SettlementToGuard)
    {
        Target = SettlementToGuard.transform;
        //set the settlement here
        SettlementBeingGuarded = SettlementToGuard;
        SettlementBeingGuarded.SettlementDestroyed += SettlementDestroyed; //Bind event
    }

    void SettlementDestroyed(Settlement DestroyedSettlement)
    {
        SettlementBeingGuarded = null;
        AttackingSettlement = false;
        Target = GameObject.Find("Player").transform;
        print("settlement destroyed");
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
    
    public void IsPartOfEnemyCamp()
    {
        PartOfEnemyCamp = true;
    }
}
