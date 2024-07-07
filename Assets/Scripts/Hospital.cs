using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Hospital : Settlement
{
    [Header("Hospital Attributes")]
    [SerializeField] private float HealTickAmount = 10;
    [SerializeField] private float TimeBeforeTick = 30;
    [SerializeField] private float MaxHealthAbleToStore = 100;
    [SerializeField] private float NoOfHealthIconAnims = 5;

    [Header("Hopsitla References")]
    [SerializeField] private Image HealthIcon;
    [SerializeField] private Animator HealthIconAnimator;


    Animator Animator;
    public float HealthAmount = 0;// { get; private set; } = 0;
    private int HealthAnimIndex = 0;
    private float AmountOfHealthBetweenAnimChanges = 0;


    // Start is called before the first frame update
    public override void Start() 
    {
        base.Start();
        HideHealthIcon();
        AmountOfHealthBetweenAnimChanges = MaxHealthAbleToStore / NoOfHealthIconAnims;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }


    public override void ClaimResources()
    {
        base.ClaimResources();
        if (HealthAmount > 0)
        {
            PlayerController.AddHealth(HealthAmount);
            HealthAmount = 0; //remove back to 0
            HealthAnimIndex = 0;
            HideHealthIcon();
        }

    }

    public override void TickResources()
    {
        base.TickResources();
        if (ResourceTimer >= TimeBeforeTick)
        {
            ResourceTimer = 0;
            HealthAmount = Mathf.Clamp(HealthAmount + HealTickAmount, 0, MaxHealthAbleToStore);
            ShowHealthIcon();
            if(HealthAmount > HealthAnimIndex * AmountOfHealthBetweenAnimChanges)
            {
                HealthIconAnimator.SetTrigger("HealthChange");
                HealthAnimIndex += 1;
            }
        }
    }

    public override void DestroySettlement()
    {
        base.DestroySettlement();
        HideHealthIcon();
    }

    private void ShowHealthIcon()
    {
        HealthIcon.enabled = true;
    }

    private void HideHealthIcon()
    {
        HealthIconAnimator.SetTrigger("HealthClaimed");
        HealthIcon.enabled = false;
    }
}
