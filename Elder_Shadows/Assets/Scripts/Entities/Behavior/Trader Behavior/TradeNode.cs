using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class TradeNode : Node
{
    private Trader trader;
    private NPC npc;

    public TradeNode(Trader trader, NPC npc)
    {
        this.trader = trader;
        this.npc = npc;
        state = NodeState.FAILURE;
    }

    public override NodeState Evaluate()
    {
        if (state != NodeState.RUNNING)
        {
            trader.StartTrading();
            state = NodeState.RUNNING;
            return state;
        }
        if (trader.IsUIClosed)
        {
            npc.FinishInteraction();
            state = NodeState.SUCCESS;
            return state;
        }
        state = NodeState.RUNNING;
        return state;
    }
}
