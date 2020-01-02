using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Agent : MonoBehaviour
{
    public int id;

    private Piece[] otherAgents;
    public Board board;
    public Vector2Int actualPos=new Vector2Int(0,0);
    public Vector2Int desiredPos = new Vector2Int(0, 0);

    //private int[,] attractivityOfCells;

    private Manager manager;
    private MailBox mailBox;


    public float speed=0.6f;
    private float  percentTraveled;
    private bool moving = false;
    private Vector3 startVector;
    private Vector2Int endCell;
    private Vector3 endVector;

    private bool isOrderedToMove; // agent received an order to move?

    const int STAY = 0;
    const int UP = 1;
    const int RIGHT = 2;
    const int DOWN = 3;
    const int LEFT = 4;
    int[] valueOfDirections = new int[5];

    public void Initialize(int id,Manager parentManager, Board boardInput,Vector2Int startPosition, Vector2Int destinationCell, MailBox mail)
    {
        this.id = id;
        this.manager = parentManager;
        this.board = boardInput;
        this.mailBox = mail;
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

    // set up movement to cell
    private void Move(Vector2Int cell)
    {
        if (board.isCellOccupied(cell)!=null)
        {
            Debug.Log("Error: trying to move to occupied cell "+cell +" by Agent n"+id);
            return;
        }

        this.percentTraveled = 0;
        this.endCell = cell;
        this.endVector = board.AgentMoveTo(this, cell);
        this.startVector = this.transform.position;
        this.moving = true;

        // just to put the object back in place if something moved it
        this.transform.position = Geometry.PointFromGrid(this.actualPos); 
    }

    private void BasicMoveTowardsDesiredPosition()
    {
        this.percentTraveled = 0;
        //this.endCell = Geometry.adjacentCellTowardsGoalCell(actualPos,desiredPos);
        Vector2Int[] poolOfMoves=board.possibleMovesOfAgent(this).ToArray();
        this.endCell = Geometry.closestCellToGoalCell(poolOfMoves, desiredPos);
        //Debug.Log(this.endCell.x+" " + this.endCell.y);
        this.endVector = board.AgentMoveTo(this, endCell);
        this.startVector = this.transform.position;
        this.moving = true;

        this.actualPos = endCell;
    }

    private void MovementDecisionWithMessages()
    {
        List<Agent> agentsOrderingToMove = mailBox.ReadMessages(id);
        isOrderedToMove = agentsOrderingToMove.Count>=1;


        List<Vector2Int> poolOfMoves = board.possibleMovesOfAgent(this);
        if (actualPos == desiredPos)
        {
            // case when no movement is required
            if (!isOrderedToMove) return;

            poolOfMoves.Remove(actualPos);
            if (poolOfMoves.Count == 0) // no adjacent cell to move to
            {
                Agent adjacentAgent = board.getAnAdjacentAgent(this);
                if (adjacentAgent != null)
                {
                    Message moveOrder = new MoveOrder(this, adjacentAgent);
                    mailBox.post(moveOrder);
                }
            }
            else
            {
                Move(poolOfMoves[0]);

            }
            return;
        }

        poolOfMoves = board.AdjacentCells(actualPos);
        foreach(Agent otherAgent in agentsOrderingToMove)
        {
            poolOfMoves.Remove(otherAgent.actualPos);
        }
        if (isOrderedToMove)
        {
            poolOfMoves.Remove(actualPos);
            //if (poolOfMoves.Count == 0)
            //{
            //    Agent adjacentAgent = board.getAnAdjacentAgent(this);
            //    if (adjacentAgent != null)
            //    {
            //        Message moveOrder = new MoveOrder(this, adjacentAgent);
            //        mailBox.post(moveOrder);
            //    }
            //    return;
            //}
            
        }
        if (poolOfMoves.Count == 0) return;
        Vector2Int selectedMove = Geometry.closestCellToGoalCell(poolOfMoves.ToArray(), desiredPos);
        if (selectedMove == actualPos) return;
        Agent possibleObstructingAgent = board.isCellOccupied(selectedMove);
        if (possibleObstructingAgent != null)
        {
            if (Random.Range(0f,1f)<0.3)
            {
                Message moveOrder = new MoveOrder(this, possibleObstructingAgent);
                mailBox.post(moveOrder);
            }
        }
        else
        {
            Move(selectedMove);
        }

    }

    //private Dictionary<Vector2Int,int> CalculateValueOfPossibleMoves(Vector2Int[] possibleMoves)
    //{

    //}

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

            //if (this.actualPos != this.desiredPos)
            //{
            //    BasicMoveTowardsDesiredPosition();
            //}
            MovementDecisionWithMessages();
        }
        //this.transform.Translate(new Vector3(1, 1, 1));
    }

    public void DebugAgent()
    {
        Debug.Log("yoooo");
    }
}
