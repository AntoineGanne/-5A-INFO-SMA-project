using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOrder : Message
{
    public MoveOrder(Agent emitter, Agent recipient) : base(emitter, recipient)
    {
    }
}
