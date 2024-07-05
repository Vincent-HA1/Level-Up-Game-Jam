using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settlement : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private float Health = 100;
    [SerializeField] private float TimeBeforeTakingDamage = 20;
    [SerializeField] private float DamagePerSecond = 3;
    [SerializeField] private float RecoveryRate = 5;

    [Header("References")]
    [SerializeField] private Slider HealthBarSlider;
    [SerializeField] private Sprite DestroyedSprite;
    [SerializeField] private Sprite UncapturedSprite;
    [SerializeField] private Sprite CapturedSprite;


    public LayerMask EnemyLayer;
    public LayerMask PlayerLayer;


    private float DamageTimer = 0;
    private bool EnemyIsGuarding = false;
    public bool IsDestroyed {  get; private set;}

    enum SettlementState{
        Uncaptured,
        Captured,
        Destroyed,
    }
    private SettlementState State = SettlementState.Uncaptured;

    private SpriteRenderer SpriteRenderer;
    private BoxCollider2D BoxCollider;

    public Action<Settlement> SettlementDestroyed;

    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        BoxCollider = GetComponent<BoxCollider2D>();
        print("Create settlement");
        HealthBarSlider.maxValue = Health;
        HealthBarSlider.value = Health;
        SpriteRenderer.sprite = UncapturedSprite;

    }

    // Update is called once per frame
    void Update()
    {
        if (State != SettlementState.Destroyed)
        {
            Collider2D EnemyOverlapped = Physics2D.OverlapCircle(transform.position, 5, EnemyLayer);
            Collider2D PlayerOverlapped = Physics2D.OverlapBox(transform.position, new Vector2(1, 1), 0, PlayerLayer);
            if (EnemyOverlapped != null)
            {
                EnemyIsGuarding = true;
            }
            else
            {
                EnemyIsGuarding = false;
                DamageTimer = 0;
            }
            if (PlayerOverlapped != null)
            {
                print(PlayerOverlapped);
                State = SettlementState.Captured;
                SpriteRenderer.sprite = CapturedSprite;
            }
            if (EnemyIsGuarding)
            {
                print("Enemy is guarding");
                DamageTimer += Time.deltaTime;
                if (DamageTimer > TimeBeforeTakingDamage)
                {
                    //start taking damage
                    Health -= Time.deltaTime * DamagePerSecond;
                    if (Health <= 0)
                    {
                        DestroySettlement();
                    }
                }
            }
            HealthBarSlider.value = Health;
        }


    }

    void DestroySettlement()
    {
        SettlementDestroyed?.Invoke(this);
        State = SettlementState.Destroyed;
        SpriteRenderer.sprite = DestroyedSprite;
        HealthBarSlider.gameObject.SetActive(false);
        BoxCollider.enabled = false;
        SpriteRenderer.color -= new Color(0.3f, 0.3f, 0.3f, 0.3f);
        //Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == PlayerLayer)
        {
            State = SettlementState.Captured;
            SpriteRenderer.sprite = CapturedSprite;
        }
    }

   
}
