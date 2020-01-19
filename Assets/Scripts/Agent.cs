using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Agent : MonoBehaviour
{
    public int id;

    // environment
    private readonly Agent[] otherAgents;
    public Board board;

    private Manager manager;
    private MailBox mailBox;

    //
    public Vector2Int actualPos=new Vector2Int(0,0);
    public Vector2Int desiredPos = new Vector2Int(0, 0);

    //private int[,] attractivityOfCells;
    
    // movement parameters
    public float speed=0.6f;
    private float  percentTraveled;
    private bool moving = false;
    private Vector3 startVector;
    private Vector2Int endCell;
    private Vector3 endVector;

    // messages
    private bool isOrderedToMove; // agent received an order to move?
    public int[] nbMoveOrderToAgents;
    public int maxMoveOrdersTogAgent=80;
    public bool waitingForAnAgentToMove=false; //wait once you emitted a move order

    // unused
    const int STAY = 0;
    const int UP = 1;
    const int RIGHT = 2;
    const int DOWN = 3;
    const int LEFT = 4;
    readonly int[] valueOfDirections = new int[5];

    public void Initialize(int id,Manager parentManager, Board boardInput,Vector2Int startPosition, Vector2Int destinationCell, MailBox mail)
    {
        this.id = id;
        this.manager = parentManager;
        this.board = boardInput;
        this.mailBox = mail;
        this.actualPos = startPosition;
        this.transform.position = board.CoordToWorld(actualPos.x, actualPos.y);
        desiredPos = destinationCell;
        this.nbMoveOrderToAgents = new int[manager.nbAgents]; 


        board.PlaceAgentOnCreation(this);
    }

    

    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = Geometry.PointFromGrid(this.actualPos);
    }

    // set up movement to cell
    private void Move(Vector2Int cell)
    {
        if (board.IsCellOccupied(cell)!=null)
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

    // select the adjacent cell closest to desired position
    private void BasicMoveTowardsDesiredPosition()
    {
        this.percentTraveled = 0;
        Vector2Int[] poolOfMoves=board.PossibleMovesOfAgent(this).ToArray();
        this.endCell = Geometry.ClosestCellToGoalCell(poolOfMoves, desiredPos);
        this.endVector = board.AgentMoveTo(this, endCell);
        this.startVector = this.transform.position;
        this.moving = true;

        this.actualPos = endCell;
    }

    private void MovementDecisionWithMessages()
    {
        List<Agent> agentsOrderingToMove = mailBox.ReadMessages(id);
        isOrderedToMove = agentsOrderingToMove.Count>=1;
        


        List<Vector2Int> poolOfMoves = board.PossibleMovesOfAgent(this);
        if (actualPos == desiredPos)
        {
            // case when no movement is required
            if (!isOrderedToMove) return;

            poolOfMoves.Remove(actualPos);
            if (poolOfMoves.Count == 0) // no adjacent cell to move to
            {
                Agent adjacentAgent = board.GetAnAdjacentAgent(this);
                if (adjacentAgent != null)
                {
                    EmitMoveOrder(adjacentAgent);
                }
            }
            else
            {
                int rdmIndex = Random.Range(0, poolOfMoves.Count);
                Move(poolOfMoves[rdmIndex]);
                SignalToAgentsWhoOrderedMeToMove(agentsOrderingToMove);
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
        }
        if (poolOfMoves.Count == 0) return;
        //Vector2Int selectedMove = Geometry.ClosestCellToGoalCell(poolOfMoves.ToArray(), desiredPos);
        List<Vector2Int> goodEnoughMoves = Geometry.CloserCellsToGoalCell(poolOfMoves.ToArray(), desiredPos, this.actualPos);
        Vector2Int freeCell= new Vector2Int(0,0);
        bool haveFoundFreeCell = false;
        foreach(Vector2Int move in goodEnoughMoves)
        {
            if (!board.IsCellOccupied(move))
            {
                freeCell = move;
                haveFoundFreeCell = true;
                break;
            }
        }
        
        
        if (haveFoundFreeCell)
        {
            if (freeCell == actualPos) return;
            Move(freeCell);
            SignalToAgentsWhoOrderedMeToMove(agentsOrderingToMove);
        }
        else
        {
            if (goodEnoughMoves.Count == 0)
            {
                Debug.Log("What");
                List<Vector2Int> poolOfMoves2 = board.AdjacentCells(actualPos);
                Vector2Int rdmCell = poolOfMoves2[Random.Range(0, poolOfMoves.Count)];
                if (board.IsCellOccupied(rdmCell)) EmitMoveOrder(rdmCell);
                else {
                    Move(rdmCell);
                    SignalToAgentsWhoOrderedMeToMove(agentsOrderingToMove);
                }
                return;
            }
            Vector2Int aGoodEnoughMove = goodEnoughMoves[Random.Range(0, goodEnoughMoves.Count)];
            Agent possibleObstructingAgent = board.IsCellOccupied(aGoodEnoughMove);
            if (possibleObstructingAgent != null)
            {
                EmitMoveOrder(possibleObstructingAgent);
            }
        }
       

    }

    private void SignalToAgentsWhoOrderedMeToMove(List<Agent> agentWhoHaveOrderedMeToMove)
    {
        foreach(Agent agent in agentWhoHaveOrderedMeToMove)
        {
            agent.OnReplyToMoveOrder();
        }
    }

    public void OnReplyToMoveOrder()
    {
        waitingForAnAgentToMove = false;
    }

    private void EmitMoveOrder(Agent recipient)
    {
        if (true || nbMoveOrderToAgents[recipient.id] <= maxMoveOrdersTogAgent )
        {
            Message moveOrder = new MoveOrder(this, recipient);
            mailBox.post(moveOrder);
            nbMoveOrderToAgents[recipient.id]++;
        }
        waitingForAnAgentToMove = true;
    }

    private void EmitMoveOrder(Vector2Int posRecipient)
    {
        Agent recipient = board.IsCellOccupied(posRecipient);
        if (recipient != null)
        {
            EmitMoveOrder(recipient);
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
            if (waitingForAnAgentToMove) return;
            //if (this.actualPos != this.desiredPos)
            //{
            //    BasicMoveTowardsDesiredPosition();
            //}
            MovementDecisionWithMessages();
        }
    }

    
}
