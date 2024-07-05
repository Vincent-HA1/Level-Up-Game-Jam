using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SettlementSquare
{
    private int MinX;
    private int MaxX;
    private int MinY;
    private int MaxY;

    public SettlementSquare(int MinX, int XSize, int MinY, int YSize)
    {
        this.MinX = MinX;
        this.MaxX = MinX + XSize;   
        this.MinY = MinY;   
        this.MaxY = MinY + YSize;
    }

    public Vector3 GetRandomPointInSquare()
    {
        return new Vector3(Random.Range(MinX, MaxX), Random.Range(MinY, MaxY));
    }
}
