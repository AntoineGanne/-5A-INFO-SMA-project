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
    public List<Agent> ReadMessages(int idAgent)
    {
        List<Agent> agentsOrderingAMove = new List<Agent>();
        List<Message> messages = mailbox[idAgent];

        foreach(Message message in messages)
        {
            agentsOrderingAMove.Add(message.emitter);

        }

        if (messages.Count >= 1)
        {
            messages.Clear();
        }
        return agentsOrderingAMove;
    }
}
