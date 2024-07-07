using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class WarningSign : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private float OffsetFromScreen = 0.05f;

    private SpriteRenderer SpriteRenderer;

    Camera MainCamera;
    Settlement SettlementToFollow;

    Transform Player;
    // Start is called before the first frame update
    void Start()
    {
        MainCamera = Camera.main;
        SpriteRenderer = GetComponent<SpriteRenderer>();
        //test code
        /*
        Player = GameObject.Find("Player").transform;
        SettlementToFollow = GameObject.Find("Hospital").GetComponent<Settlement>();
        */
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Player)
        {
            //check if settlement is in screen. If so, hide for a bit. 
            if (PositionInCameraBounds(SettlementToFollow.transform.position, -OffsetFromScreen * 2)) //negative because we want to hide it just before it comes on screen
            {
                SpriteRenderer.enabled = false;
            }
            else
            {
                //Only move the warning and show it when the settlement is offscreen
                if (!SpriteRenderer.enabled)
                {
                    SpriteRenderer.enabled = true;
                }
                //Move the warning sign in the direction of the settlement from the player. Move it until the edge of the screen
                Vector2 Direction = (SettlementToFollow.transform.position - Player.position).normalized;
                Vector3 Offset = FindMaxMagnitude(Direction, Player.position);

                transform.position = Player.position + Offset;
            }

        }

    }

    Vector3 FindMaxMagnitude(Vector3 direction, Vector3 startPoint)
    {
        //binary search. Divide by 2 until the max and min are around the same (i.e. we've found the correct amount)
        float minMultiplier = 0;
        float maxMultiplier = 10f; // Initial guess, can be larger if needed
        float tolerance = 0.01f; // Tolerance for stopping the binary search

        Vector3 bestPoint = startPoint;

        while ((maxMultiplier - minMultiplier) > tolerance)
        {
            float midMultiplier = (minMultiplier + maxMultiplier) / 2;
            Vector3 testPoint = startPoint + midMultiplier * direction;

            if (PositionInCameraBounds(testPoint, OffsetFromScreen))
            {
                bestPoint = testPoint;
                minMultiplier = midMultiplier;
            }
            else
            {
                maxMultiplier = midMultiplier;
            }
        }

        return bestPoint - startPoint;
    }

    bool PositionInCameraBounds(Vector3 Position, float Offset = 0)
    {
        // Convert object's position to viewport space
        Vector3 ViewportPos = MainCamera.WorldToViewportPoint(Position);

        // Check if object is within camera bounds
        if (ViewportPos.x >= Offset && ViewportPos.x <= 1 - Offset &&
            ViewportPos.y >= Offset && ViewportPos.y <= 1- Offset &&
            ViewportPos.z > 0) // Ensure the object is in front of the camera
        {
            return true;
        }
        return false;
    }
    public void SetSettlementToFollow(Settlement SettlementToFollow, Transform Player)
    {
        this.SettlementToFollow = SettlementToFollow;
        SettlementToFollow.SettlementIsSafe += DestroySelf; //destroy warning sign when 
        SettlementToFollow.SettlementDestroyed += DestroySelf; //destroy warning sign when 
        this.Player = Player;
    }

    void DestroySelf(Settlement Settlement)
    {
        Destroy(gameObject);
    }
}
