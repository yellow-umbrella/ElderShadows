using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BehaviorTree
{
    public class SequenceNode : Node
    {
        public SequenceNode() : base() { }
        public SequenceNode(List<Node> children) : base(children) { }

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
                        state = NodeState.FAILURE;
                        SetData(PREV_ACTION, null);
                        return state;
                    case NodeState.SUCCESS:
                        continue;
                    case NodeState.RUNNING:
                        state = NodeState.RUNNING;
                        SetData(PREV_ACTION, node.GetData(PREV_ACTION));
                        return state;
                }
            }

            if (children.Count != 0)
            {
                SetData(PREV_ACTION, children[children.Count - 1].GetData(PREV_ACTION));
            } else
            {
                SetData(PREV_ACTION, this);
            }
            
            state = NodeState.SUCCESS;
            return state;
        }
    }

}
