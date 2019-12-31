using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    private Piece[] otherAgents;
    public Board board;
    public Vector2Int actualPos=new Vector2Int(0,0);
    public Vector2Int desiredPos = new Vector2Int(0, 0);

    //private int[,] attractivityOfCells;

    private Manager manager;


    public float speed=0.6f;
    private float  percentTraveled;
    private bool moving = false;
    private Vector3 startVector;
    private Vector2Int endCell;
    private Vector3 endVector;

    public void Initialize(Manager parentManager, Board boardInput,Vector2Int startPosition, Vector2Int destinationCell)
    {
        this.manager = parentManager;
        this.board = boardInput;
        this.actualPos = startPosition;
        this.transform.position = board.CoordToWorld(actualPos.x, actualPos.y);
        desiredPos = destinationCell;

        board.placeAgentOnCreation(this);
    }

    

    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = Geometry.PointFromGrid(this.actualPos);
    }

    private void Move()
    {
        this.percentTraveled = 0;
        this.endCell = desiredPos;
        this.endVector = board.AgentMoveTo(this, desiredPos);
        this.startVector = this.transform.position;
        this.moving = true;
        this.transform.position = Geometry.PointFromGrid(this.actualPos);

        
    }

    private void BasicMoveTowardsDesiredPosition()
    {
        this.percentTraveled = 0;
        //this.endCell = Geometry.adjacentCellTowardsGoalCell(actualPos,desiredPos);
        Vector2Int[] poolOfMoves=board.possibleMovesOfAgent(this);
        this.endCell = Geometry.closestCellToGoalCell(poolOfMoves, desiredPos);
        //Debug.Log(this.endCell.x+" " + this.endCell.y);
        this.endVector = board.AgentMoveTo(this, endCell);
        this.startVector = this.transform.position;
        this.moving = true;

        this.actualPos = endCell;
    }

    private void IsDoneMoving()
    {
        moving = false;
        //board.FreeCell(actualPos);
        this.actualPos = endCell;

    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            this.percentTraveled = percentTraveled+speed*Time.deltaTime;
            this.transform.position = Vector3.Lerp(startVector, endVector, percentTraveled);
            if (percentTraveled>=1)
            {
                IsDoneMoving();
                
            }
        }
        else
        {
            if (this.actualPos != this.desiredPos)
            {
                BasicMoveTowardsDesiredPosition();
            }
        }
        //this.transform.Translate(new Vector3(1, 1, 1));
    }

    public void DebugAgent()
    {
        Debug.Log("yoooo");
    }
}
