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
                SetData(PREV_ACTION, parent.GetData(PREV_ACTION));
            }

            foreach (Node node in children)
            {
                switch (node.Evaluate())
                {
                    case NodeState.FAILURE:
                        continue;
                    case NodeState.SUCCESS:
                        state = NodeState.SUCCESS;
                        SetData(PREV_ACTION, node.GetData(PREV_ACTION));
                        return state;
                    case NodeState.RUNNING:
                        SetData(PREV_ACTION, node.GetData(PREV_ACTION));
                        state = NodeState.RUNNING;
                        return state;
                }
            }

            SetData(PREV_ACTION, null);
            state = NodeState.FAILURE;
            return state;
        }
    }

}
