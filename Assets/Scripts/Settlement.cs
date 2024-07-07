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
    [SerializeField] private float BaseDamagePerSecond = 3;
    [SerializeField] private float MaxDamagePerSecond = 12;
    [SerializeField] private float RecoveryRate = 5;

    [Header("References")]
    [SerializeField] private Slider HealthBarSlider;
    [SerializeField] private Sprite DestroyedSprite;
    [SerializeField] private Sprite UncapturedSprite;
    [SerializeField] private Sprite CapturedSprite;


    public LayerMask EnemyLayer;
    public LayerMask PlayerLayer;


    private float DamageTimer = 0;
    private bool EnemyIsAttacking = false;
    [SerializeField] private float NumberOfEnemiesAttacking = 0;
    public bool IsDestroyed {  get; private set;}
    public bool IsCaptured { get; private set; }

    //Reference to player
    protected PlayerController PlayerController;

    protected float ResourceTimer = 0;


    enum SettlementState
    {
        Uncaptured,
        Captured,
        Destroyed,
    }
    private SettlementState State = SettlementState.Uncaptured;

    private SpriteRenderer SpriteRenderer;
    private BoxCollider2D BoxCollider;

    public Action<Settlement> SettlementDestroyed;
    public Action<Settlement> SettlementAttacked;
    public Action<Settlement> SettlementIsSafe;



    // Start is called before the first frame update
    public virtual void Start()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        BoxCollider = GetComponent<BoxCollider2D>();
        print("Create settlement");
        HealthBarSlider.maxValue = Health;
        HealthBarSlider.value = Health;
        SpriteRenderer.sprite = UncapturedSprite;

    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (State != SettlementState.Destroyed)
        {
            //CheckForEnemy();
            CheckForPlayer();
            TakeDamage();
            if (State == SettlementState.Captured && !EnemyIsAttacking)
            {
                TickResources();
            }
        }


    }

    private void TakeDamage()
    {
        if (EnemyIsAttacking)
        {
            DamageTimer += Time.deltaTime;
            if (DamageTimer > TimeBeforeTakingDamage)
            {
                float DamagePerSecond = Mathf.Clamp(BaseDamagePerSecond * NumberOfEnemiesAttacking, 0, MaxDamagePerSecond);
                print(DamagePerSecond);
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

    public void AddEnemyAttacker()
    {
        NumberOfEnemiesAttacking += 1;
        if (EnemyIsAttacking)
        {
            SettlementAttacked?.Invoke(this);
        }
        EnemyIsAttacking= true;
    }

    public void RemoveEnemyAttacker()
    {
        NumberOfEnemiesAttacking -= 1;
        if(NumberOfEnemiesAttacking <= 0)
        {
            EnemyIsAttacking = false;
            SettlementIsSafe?.Invoke(this);
        }
    }

    public virtual void DestroySettlement()
    {
        IsDestroyed = true;
        SettlementDestroyed?.Invoke(this);
        State = SettlementState.Destroyed;
        SpriteRenderer.sprite = DestroyedSprite;
        HealthBarSlider.gameObject.SetActive(false);
        BoxCollider.enabled = false;
        SpriteRenderer.color -= new Color(0.3f, 0.3f, 0.3f, 0.3f);
        //Destroy(gameObject);
    }


    void CheckForPlayer()
    {
        //Can only interact with settlement if it isn't being attacked
        if (!EnemyIsAttacking)
        {
            Collider2D PlayerOverlapped = Physics2D.OverlapBox(transform.position, new Vector2(1, 1), 0, PlayerLayer);
            if (PlayerOverlapped)
            {
                if (!PlayerController)
                {
                    PlayerController = PlayerOverlapped.GetComponent<PlayerController>();
                }
                if (State != SettlementState.Captured)
                {
                    State = SettlementState.Captured;
                    SpriteRenderer.sprite = CapturedSprite;
                    IsCaptured = true;
                }
                else
                {
                    ClaimResources();
                }
            }
        }


    }

    //Settlement specific function for adding resources to the player
    public virtual void ClaimResources()
    {
        ResourceTimer = 0;
    }

    //Settlement specific function for accumulat
    public virtual void TickResources()
    {
        ResourceTimer += Time.deltaTime;
    }


}
