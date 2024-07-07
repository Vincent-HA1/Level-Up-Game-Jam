using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoldierCamp : Settlement
{
    [Header("Soldier Attributes")]
    [SerializeField] private float SoldierIncreaseAmountPerTick = 1;
    [SerializeField] private float TimeBeforeTick = 20;
    [SerializeField] private float MaxSoldiers = 10;

    [Header("Soldier References")]
    [SerializeField] private Image SoldierImage;
    [SerializeField] private TMPro.TextMeshProUGUI SoldierNumber;


    public float SoldierAmount = 0;//{ get; private set; } = 0;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        HideSoldierAmount();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    public override void ClaimResources()
    {
        base.ClaimResources();
        if (SoldierAmount > 0)
        {
            PlayerController.AddSoldiers(SoldierAmount);
            SoldierAmount = 0; //remove back to 0
            HideSoldierAmount();
        }

    }

    public override void TickResources()
    {
        base.TickResources();
        if(ResourceTimer >= TimeBeforeTick)
        {
            print(ResourceTimer);
            print("show soldier amount");
            ResourceTimer = 0;
            SoldierAmount = Mathf.Clamp(SoldierAmount + SoldierIncreaseAmountPerTick, 0, MaxSoldiers);
            ShowSoldierAmount();
        }
    }

    public override void DestroySettlement()
    {
        base.DestroySettlement();
        HideSoldierAmount();
    }

    private void ShowSoldierAmount()
    {

        SoldierImage.enabled = true;
        SoldierNumber.enabled = true;
        SoldierNumber.text = "x " + SoldierAmount.ToString();
    }

    private void HideSoldierAmount()
    {
        SoldierImage.enabled = false;
        SoldierNumber.enabled = false;
    }
}
