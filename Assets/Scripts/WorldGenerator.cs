using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGenerator : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private float GenerationSize = 500;
    [SerializeField] private int BorderSize = 50;
    [SerializeField] private int MinNumberOfSettlements = 10;
    [SerializeField] private int MaxNumberOfSettlements = 20;
    [SerializeField] private int MinNumberOfHospitals = 10;
    [SerializeField] private int MaxNumberOfHospitals = 20;
    [SerializeField] private int MinDistanceBetweenSettlementSquares = 20;


    [Header("References")]
    [SerializeField] private Tilemap GrassTilemap;
    [SerializeField] private Tile GrassTile;
    [SerializeField] private GameObject SettlementPrefab;
    [SerializeField] private GameObject HospitalPrefab;
    [SerializeField] private GameObject BossCastlePrefab;
    [SerializeField] private GameObject BorderPrefab;
    [SerializeField] private GameObject Player;

    private List<SettlementSquare> SettlementSquares = new List<SettlementSquare>();

    public List<Settlement> UndestroyedSettlements = new List<Settlement>();

    // Start is called before the first frame update
    void Awake()
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
        for (int x = MinX - BorderSize/2; x < MaxX + BorderSize/2; x++)
        {
            for (int y = MinY - BorderSize / 2; y < MaxY + BorderSize / 2; y++)
            {
                GrassTilemap.SetTile(new Vector3Int(x, y), GrassTile);
            }
        }
        BorderSize /= 2;
        //Create Borders. Instantiate borders
        GameObject BorderBottom = Instantiate(BorderPrefab, new Vector3(0, MinY), Quaternion.identity, null);
        BorderBottom.GetComponent<BoxCollider2D>().size = new Vector2(GenerationSize + BorderSize, BorderSize);
        BorderBottom.GetComponent<BoxCollider2D>().offset = new Vector2(0, -BorderSize / 2);
        GameObject BorderTop = Instantiate(BorderPrefab, new Vector3(0, MaxY), Quaternion.identity, null);
        BorderTop.GetComponent<BoxCollider2D>().size = new Vector2(GenerationSize + BorderSize, BorderSize);
        BorderTop.GetComponent<BoxCollider2D>().offset = new Vector2(0, BorderSize / 2);
        GameObject BorderLeft = Instantiate(BorderPrefab, new Vector3(MinX, 0), Quaternion.identity, null);
        BorderLeft.GetComponent<BoxCollider2D>().size = new Vector2(BorderSize, GenerationSize + BorderSize);
        BorderLeft.GetComponent<BoxCollider2D>().offset = new Vector2(-BorderSize / 2, 0);
        GameObject BorderRight = Instantiate(BorderPrefab, new Vector3(MaxX, 0), Quaternion.identity, null);
        BorderRight.GetComponent<BoxCollider2D>().size = new Vector2(BorderSize, GenerationSize + BorderSize);
        BorderRight.GetComponent<BoxCollider2D>().offset = new Vector2(BorderSize / 2, 0);

        int NumberOfSettlementsToSpawn = Random.Range(MinNumberOfSettlements, MaxNumberOfSettlements);
        int NumberOfHospitalsToSpawn = Random.Range(MinNumberOfHospitals, MaxNumberOfHospitals);
        //create grid
        CreateGrid(MinX, MaxX, MinY, MaxY, NumberOfSettlementsToSpawn + NumberOfHospitalsToSpawn);
        //Test spawn settlements
        //Spawn settlements
        //Spawning soldier camps here
        for (int i = 0; i < NumberOfSettlementsToSpawn; i++)
        {
            int RandomSettlementSquare = Random.Range(0, SettlementSquares.Count - 1);
            //choose random free spot within the settlement square
            Vector3 SpawnPoint = SettlementSquares[RandomSettlementSquare].GetRandomPointInSquare();
            SettlementSquares.Remove(SettlementSquares[RandomSettlementSquare]); //remove the square to prevent it from being used
            //Spawn the settlement
            Settlement NewSettlement = Instantiate(SettlementPrefab, SpawnPoint, Quaternion.identity, null).GetComponent<Settlement>();
            UndestroyedSettlements.Add(NewSettlement);
            NewSettlement.SettlementDestroyed += DestroySettlement;
        }
        //spawning hopsitals here
        for (int i = 0; i < NumberOfHospitalsToSpawn; i++)
        {
            int RandomSettlementSquare = Random.Range(0, SettlementSquares.Count - 1);
            //choose random free spot within the settlement square
            Vector3 SpawnPoint = SettlementSquares[RandomSettlementSquare].GetRandomPointInSquare();
            SettlementSquares.Remove(SettlementSquares[RandomSettlementSquare]); //remove the square to prevent it from being used
            //Spawn the settlement
            Settlement NewHospital = Instantiate(HospitalPrefab, SpawnPoint, Quaternion.identity, null).GetComponent<Settlement>();
            UndestroyedSettlements.Add(NewHospital);
            NewHospital.SettlementDestroyed += DestroySettlement;
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
