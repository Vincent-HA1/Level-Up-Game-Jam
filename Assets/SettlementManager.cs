using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettlementManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] WorldGenerator WorldGenerator;
    [SerializeField] GameObject WarningSignPrefab;
    [SerializeField] Transform Player;
    // Start is called before the first frame update
    void Start()
    {
        foreach (Settlement SettlementToMonitor in WorldGenerator.UndestroyedSettlements)
        {
            SettlementToMonitor.SettlementAttacked += SettlementAttacked;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SettlementAttacked(Settlement SettlementThatIsBeingAttacked)
    {
        //spawn a warning sign that follows the settlement in the durection
        GameObject WarningSign = Instantiate(WarningSignPrefab, null);
        WarningSign.GetComponent<WarningSign>().SetSettlementToFollow(SettlementThatIsBeingAttacked, Player);
    }
}
