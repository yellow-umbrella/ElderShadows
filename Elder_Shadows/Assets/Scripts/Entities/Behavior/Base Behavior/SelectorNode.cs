using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public class SelectorNode : Node
    {
        public SelectorNode():base() { }
        public SelectorNode(List<Node> children) : base(children) { }

        public override NodeState Evaluate()
        {
            if (parent != null)
            {
                if (parent.GetData(PREV_ACTION_CHILD) != this) 
                    SetData(PREV_ACTION_CHILD, null);
            }
            SetData(RUNNING_ACTION, null);
            foreach (Node node in children)
            {
                switch (node.Evaluate())
                {
                    case NodeState.FAILURE:
                        continue;
                    case NodeState.SUCCESS:
                        state = NodeState.SUCCESS;
                        SetData(PREV_ACTION_CHILD, node);
                        return state;
                    case NodeState.RUNNING:
                        SetData(PREV_ACTION_CHILD, node);
                        SetData(RUNNING_ACTION, node.GetData(RUNNING_ACTION));
                        state = NodeState.RUNNING;
                        return state;
                }
            }

            SetData(PREV_ACTION_CHILD, null);
            state = NodeState.FAILURE;
            return state;
        }
    }

}
