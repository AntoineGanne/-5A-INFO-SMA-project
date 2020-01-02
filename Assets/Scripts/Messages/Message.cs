using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: génerer des heritiers de la classe pour les differentes request?
public enum RequestType {Question,MoveOrder}

public class Message
{
    public Agent emitter;
    public Agent recipient; //destinataire

    //private string requestType;
    //private Vector2Int concernedCell;

    public Message(Agent emitter, Agent recipient)
    {
        this.emitter = emitter;
        this.recipient = recipient;
    }

    public void applyValue(Dictionary<Vector2Int, int> valueOfPossibleMoves)
    {
        //valueOfPossibleMoves[concernedCell] -= 5;
        //Debug.Log("moveOrder took place for cell:" + concernedCell);
    }
}
