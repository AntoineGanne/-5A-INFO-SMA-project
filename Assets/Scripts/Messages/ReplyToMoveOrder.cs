using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplyToMoveOrder : Message
{
    public ReplyToMoveOrder(Agent emitter, Agent recipient) : base(emitter, recipient)
    {
    }
}
