using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public static Manager instance;

    public Board board;
    
    public GameObject[] possibleGameObjects;

    /** we store which cells are still avalaible to setup our agents.
     *  Less complexity than using randomness.
     *  we store Integers to lower memory complexity.
     **/
    private ArrayList availableFreeCells;
    private ArrayList availableDestinationCells;

   
    public Agent[] agents;
    public int NbAgents=3;

    void Awake()
    {
        instance = this;
        
    }

    void Start()
    {
        availableFreeCells = new ArrayList(board.size * board.size);
        availableDestinationCells = new ArrayList(board.size * board.size);
        for(int x = 0; x < board.size; ++x)
        {
            for(int y = 0; y < board.size; ++y)
            {
                int cellIndex = x*board.size + y;
                availableFreeCells.Add(cellIndex);
                availableDestinationCells.Add(cellIndex);
            }
        }
        InitialSetup();
    }

    private GameObject getRandomGameObject()
    {
        int randomIndex = Random.Range(0, possibleGameObjects.Length);
        return possibleGameObjects[randomIndex];
    }

    private void InitialSetup()
    {
        agents = new Agent[NbAgents];
      
        for (int i = 0; i < NbAgents; ++i)
        {
            //agents[i]= Instantiate(getRandomGameObject())
       
            GameObject agentObject = Instantiate(getRandomGameObject());
            agentObject.AddComponent(typeof(Agent));

            Vector2Int startPos = findFreeCell();
            Vector2Int destinationPos = findDestinationcell();

            Agent agent = agentObject.GetComponent<Agent>();
            agent.Initialize(this, board,startPos, destinationPos);
            agents[i] = agent;
        }

    }

    /** 
     *  get a random avalaible vector from avalaibleFreeCell.
     *  remove the vector from arraylist.
     *  Return: randomly selected vector
     **/
    private Vector2Int findFreeCell()
    {
        int randomIndex = Random.Range(0, availableFreeCells.Count);
        int? cell = availableFreeCells[randomIndex] as int?;
        availableFreeCells.RemoveAt(randomIndex);
        if (cell is int indexCell)
        {
            return Geometry.intToVector2int(indexCell, board.size);
        }
        else
        {
            Debug.Log("error: getting null value from arraylist avalaibleFreeCell");
            return new Vector2Int(0, 0);
        }
    }

    /** 
     *  get a random avalaible vector from avalaibleDestinationCell.
     *  remove the vector from arraylist.
     *  Return: randomly selected vector
     **/
    private Vector2Int findDestinationcell()
    {
        int randomIndex = Random.Range(0, availableDestinationCells.Count);
        int? cell = availableDestinationCells[randomIndex] as int?;
        availableDestinationCells.RemoveAt(randomIndex);
        if (cell is int indexCell)
        {
            return Geometry.intToVector2int(indexCell,board.size);
        }
        else
        {
            Debug.Log("error: getting null value from arraylist avalaibleFreeCell");
            return new Vector2Int(0, 0);
        }
    }



    public void AddPiece(GameObject prefab, int col, int row)
    {
       
    }
}
