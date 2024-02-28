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

        private int runningInd = 0;

        public override NodeState Evaluate()
        {
            if (parent != null)
            {
                if (parent.GetData(PREV_ACTION_CHILD) != this)
                {
                    SetData(PREV_ACTION_CHILD, null);
                    runningInd = 0;
                }
            }
            SetData(RUNNING_ACTION, null);
            for (int i = runningInd; i < children.Count; i++)
            {
                switch (children[i].Evaluate())
                {
                    case NodeState.FAILURE:
                        state = NodeState.FAILURE;
                        SetData(PREV_ACTION_CHILD, null);
                        return state;
                    case NodeState.SUCCESS:
                        continue;
                    case NodeState.RUNNING:
                        state = NodeState.RUNNING;
                        SetData(PREV_ACTION_CHILD, children[i]);
                        SetData(RUNNING_ACTION, children[i].GetData(RUNNING_ACTION));
                        runningInd = i;
                        return state;
                }
            }

            runningInd = 0;
            if (children.Count != 0)
            {
                SetData(PREV_ACTION_CHILD, children[children.Count - 1]);
            } else
            {
                SetData(PREV_ACTION_CHILD, this);
            }
            
            state = NodeState.SUCCESS;
            return state;
        }
    }

}
