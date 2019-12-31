using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: génerer des heritiers de la classe pour les differentes request?
public enum RequestType {Question,Order}

public class Message: MonoBehaviour
{
    private Agent emitter;
    private Agent recipient; //destinataire

    private string requestType;
    private Vector2Int desiredPos;
    

   
}
