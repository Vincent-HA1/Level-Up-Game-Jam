using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Attributes")]
    [SerializeField] private float MoveSpeed; 

    private Rigidbody2D Rigid;

    //Test variables
    public float SoldiersAmount = 0;
    public float Health = 100;
    private float MaxHealth = 100;

    // Start is called before the first frame update
    void Start()
    {
        Rigid = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Rigid.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * MoveSpeed;
    }

    public void AddHealth(float HealthAmount)
    {
        //do what you want here
        Health = Mathf.Clamp(Health + HealthAmount, 0 , MaxHealth);
    }

    public void AddSoldiers(float SoldiersAmount)
    {
        //do what you want here
        this.SoldiersAmount += SoldiersAmount;
    }
}
