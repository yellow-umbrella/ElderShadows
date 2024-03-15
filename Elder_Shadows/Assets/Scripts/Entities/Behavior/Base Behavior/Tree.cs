using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public abstract class Tree : MonoBehaviour
    {
        public Node ActiveNode { get { return root.GetData(Node.RUNNING_ACTION) as Node; } }
        protected Node root = null;

        private void Start()
        {
            root = SetupTree();
        }

        protected virtual void Update()
        {
            if (root != null)
            {
                root.Evaluate();
            }
        }

        protected abstract Node SetupTree();
    }

}
