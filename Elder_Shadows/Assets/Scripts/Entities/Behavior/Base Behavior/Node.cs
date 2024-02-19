using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BehaviorTree
{
    public enum NodeState
    {
        RUNNING,
        SUCCESS,
        FAILURE
    }

    public class Node
    {
        public Node parent;
        protected List<Node> children = new List<Node>();

        protected NodeState state;
        private Dictionary<string, object> data = new Dictionary<string, object>();

        protected const string PREV_ACTION = "prev_success";

        public Node() 
        {
            parent = null;
        }

        public Node(List<Node> children)
        {
            foreach (Node child in children)
            {
                Attach(child);
                child.SetData(PREV_ACTION, child);
            }
        }

        private void Attach(Node node)
        {
            node.parent = this;
            children.Add(node);
        }

        public virtual NodeState Evaluate() => NodeState.FAILURE;

        public object GetData(string key)
        {
            if (data.TryGetValue(key, out object value))
            {
                return value;
            }

            if (parent == null)
            {
                return null;
            }

            return parent.GetData(key);
        }

        public void SetData(string key, object value)
        {
            data[key] = value;
        }

        public bool ClearData(string key)
        {
            if (data.ContainsKey(key))
            {
                data.Remove(key);
                return true;
            }

            if (parent == null)
            {
                return false;
            }

            return parent.ClearData(key);
        }

        public Node GetRoot()
        {
            Node root = this;
            while (root.parent != null)
            {
                root = root.parent;
            }
            return root;
        }
    }

}
