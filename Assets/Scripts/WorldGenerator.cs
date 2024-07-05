using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGenerator : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private float GenerationSize = 500;
    [SerializeField] private int MinNumberOfSettlements = 10;
    [SerializeField] private int MaxNumberOfSettlements = 20;
    [SerializeField] private int MinDistanceBetweenSettlementSquares = 20;


    [Header("References")]
    [SerializeField] private Tilemap GrassTilemap;
    [SerializeField] private Tile GrassTile;


    [SerializeField] private GameObject SettlementPrefab;
    [SerializeField] private GameObject HospitalPrefab;
    [SerializeField] private GameObject BossCastlePrefab;
    [SerializeField] private GameObject Player;

    private List<SettlementSquare> SettlementSquares = new List<SettlementSquare>();

    public List<Settlement> UndestroyedSettlements = new List<Settlement>();

    // Start is called before the first frame update
    void Start()
    {
        GenerateWorld();
    }

    void GenerateWorld()
    {
        int MinX = (int)(transform.position.x - GenerationSize / 2);
        int MinY = (int)(transform.position.y - GenerationSize / 2);
        int MaxX = (int)(transform.position.x + GenerationSize / 2);
        int MaxY = (int)(transform.position.y + GenerationSize / 2);


        //start in transform.position
        //Spawn all starting tiles
        for (int x = MinX; x < MaxX; x++)
        {
            for (int y = MinY; y < MaxY; y++)
            {
                GrassTilemap.SetTile(new Vector3Int(x, y), GrassTile);
            }
        }
        int NumberOfSettlementsToSpawn = Random.Range(MinNumberOfSettlements, MaxNumberOfSettlements);
        int SettlementsSpawned = 0;
        //create grid
        CreateGrid(MinX, MaxX, MinY, MaxY, NumberOfSettlementsToSpawn);
        //Test spawn settlements
        //Spawn settlements
        for (int i = 0; i < NumberOfSettlementsToSpawn; i++)
        {
            //choose random free spot within the settlement square
            Vector3 SpawnPoint = SettlementSquares[i].GetRandomPointInSquare();
            //Spawn the settlement
            Settlement NewSettlement = Instantiate(SettlementPrefab, SpawnPoint, Quaternion.identity, null).GetComponent<Settlement>();
            UndestroyedSettlements.Add(Instantiate(SettlementPrefab, SpawnPoint, Quaternion.identity, null).GetComponent<Settlement>());
            NewSettlement.SettlementDestroyed += DestroySettlement;
            SettlementsSpawned++;
        }

        Player.transform.position = new Vector3(transform.position.x, transform.position.y, Player.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DestroySettlement(Settlement DestroyedSettlement)
    {
        UndestroyedSettlements.Remove(DestroyedSettlement);
    }

    void CreateGrid(int MinX, int MaxX, int MinY, int MaxY, int NoOfSettlementsToSpawn)
    {
        //There needs to be x * x number of squares, where x * x = Number of settleemnts
        //round to the biggest nearest whole number
        int NumberOfSquaresInARow = Mathf.CeilToInt(Mathf.Sqrt(NoOfSettlementsToSpawn));
        int NumberOfSquaresInAColumn = NumberOfSquaresInARow; //For now, the columsn and rows will have the same number
        //We know how many squares tehre will be in x and y (root(NumberOfSquares)
        //Square size can be found by how many squares in X, - the min distance
        int SquareSize = Mathf.CeilToInt(GenerationSize / NumberOfSquaresInARow) - MinDistanceBetweenSettlementSquares;
        int SquareSizeOneWay = SquareSize;// Mathf.FloorToInt(Mathf.Sqrt(SquareSize));
        print(GenerationSize);
        print(SquareSize);
        for (int i = 0; i < NumberOfSquaresInAColumn; i++)
        {
            for(int j = 0; j < NumberOfSquaresInARow; j++)
            {
                int SquareOffset = SquareSizeOneWay + MinDistanceBetweenSettlementSquares; //add the gap to start the min
                SettlementSquare NewSettlementSquare = new SettlementSquare(MinX + SquareOffset * j, SquareSizeOneWay, MinY + SquareOffset * i, SquareSizeOneWay);
                SettlementSquares.Add(NewSettlementSquare);
            }
        }

    }

}
