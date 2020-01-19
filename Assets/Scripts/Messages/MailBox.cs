using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MailBox 
{
    Dictionary<int, List<Message>> mailbox; // key = id of agent , value=unread messages meant for agent

    public MailBox(int nbAgent)
    {
        mailbox = new Dictionary<int, List<Message>>();
        for(int i = 0; i < nbAgent; ++i)
        {
            List<Message> list = new List<Message>();
            mailbox.Add(i, list);
        }
    }

    public void post(Message message)
    {
        int recipient = message.recipient.id;
        List < Message> messages= mailbox[recipient];
        messages.Add(message);
    }

    /** return list of agents requiring current agent to move
     * 
     * */
    public List<Agent> ReadMoveOrders(int idAgent)
    {
        List<Agent> agentsOrderingAMove = new List<Agent>();
        List<Message> messages = mailbox[idAgent];
        List<Message> messagesRead = new List<Message>();

        foreach(Message message in messages)
        {
            if(message is MoveOrder)
            {
                agentsOrderingAMove.Add(message.emitter);
                messagesRead.Add(message);
            }
        }
        foreach(Message msg in messagesRead)
        {
            messages.Remove(msg);
        }

        //if (messages.Count >= 1)
        //{
        //    messages.Clear();
        //}
        return agentsOrderingAMove;
    }

    public bool readRepliesToMoveOrder(int idAgent)
    {
        List<Message> messages = mailbox[idAgent];
        List<Message> messagesRead = new List<Message>();
        bool hadAReply = false;

        foreach (Message message in messages)
        {
            if (message is ReplyToMoveOrder)
            {
                messagesRead.Add(message);
                hadAReply = true;
            }
        }
        foreach (Message msg in messagesRead)
        {
            messages.Remove(msg);
        }
        return hadAReply;
    }
}
