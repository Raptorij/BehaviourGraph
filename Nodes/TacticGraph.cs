using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace RaptorijDevelop.BehaviourGraph
{
    [CreateAssetMenu( menuName = "Tactic Graph/Tactic Graph", fileName = "New TacticGroup")]
    public class TacticGraph : ScriptableObject
    {
        public bool IsActive => deactivators.Count == 0;

        private HashSet<Object> deactivators = new HashSet<Object>();

        public Node rootNode;
        public Node currentNode;
        bool isStarted = false;
        public TacticDirector tacticDirector;
        public Blackboard blackboard;
        public Node.State graphState = Node.State.Running;
        public Rect rect;
        public List<Node> nodes = new List<Node>();

        public void AddDeactivator(Object obj)
        {
            if (deactivators.Add(obj))
            {
                if (currentNode != rootNode && currentNode != null && currentNode.started)
                {
                    (currentNode as IActionNode).OnStop();
                }
            }
        }

        public void RemoveDeactivator(Object obj)
        {
            if (deactivators.Remove(obj))
            {
                SetCurrentNode(rootNode);
            }
        }

        public void Initialize(TacticDirector animationDirector)
        {
            this.tacticDirector = animationDirector;
            if (animationDirector.TryGetComponent<Blackboard>(out var blackboard))
            {
                this.blackboard = blackboard;
            }
            rootNode.Initialize(this);
            SetCurrentNode(rootNode);
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Initialize(this);
            }
            isStarted = true;
        }

        public Node.State Update()
        {
            if (isStarted && IsActive)
            {
                graphState = currentNode.Update();
            }
            return graphState;
        }

        public void SetCurrentNode(Node node)
        {
            if (IsActive)
            {
                if (currentNode is IActionNode actionNode && currentNode.started)
                {
                    actionNode.OnStop();
                }
                currentNode = node;
            }
        }

#if UNITY_EDITOR
        public Node CreateNode(System.Type type)
        {
            Node node = ScriptableObject.CreateInstance(type) as Node;
            node.name = type.Name;
            if (node is BehaviourNode tacticNode)
            {
                tacticNode.nodeName = $"New{type.Name}";
            }
            node.guid = GUID.Generate().ToString();
            node.graph = this;
            nodes.Add(node);

            AssetDatabase.AddObjectToAsset(node, this);
            AssetDatabase.SaveAssets();

            return node;
        }

        public void DeleteNode(Node node)
        {
            nodes.Remove(node);
            AssetDatabase.RemoveObjectFromAsset(node);
            AssetDatabase.SaveAssets();
        }

        public Transition AddConnection(Node parent, Node connection)
        {
            if (parent is BehaviourNode tacticNode)
            {
                tacticNode.connections.Add(connection);

                Transition transition = ScriptableObject.CreateInstance(typeof(Transition)) as Transition;
                transition.graph = this;
                transition.name = $"{parent.name}=>{connection.name}";
                transition.guid = GUID.Generate().ToString();
                transition.parent = tacticNode;
                transition.connection = connection as BehaviourNode;
                tacticNode.transitions.Add(transition);
                AssetDatabase.AddObjectToAsset(transition, this);
                AssetDatabase.SaveAssets();
                return transition;
            }

            EnterNode enterNode = parent as EnterNode;
            if (enterNode != null)
            {
                enterNode.connection = connection;
                Transition transition = ScriptableObject.CreateInstance(typeof(Transition)) as Transition;
                transition.graph = this;
                transition.name = $"{parent.name}=>{connection.name}";
                transition.guid = GUID.Generate().ToString();
                transition.parent = enterNode;
                transition.connection = connection as BehaviourNode;
                enterNode.transition = transition;
                AssetDatabase.AddObjectToAsset(transition, this);
                AssetDatabase.SaveAssets();
                return transition;
            }

            return null;
        }

        public void RemoveConnection(Node parent, Node connection)
        {
            if (parent is BehaviourNode tacticNode)
            {
                tacticNode.connections.Remove(connection);
            }

            EnterNode enterNode = parent as EnterNode;
            if (enterNode != null && connection == enterNode.connection)
            {
                enterNode.connection = null;
            }
        }

        public void RemoveTransition(Transition transition)
        {
            RemoveConnection(transition.parent, transition.connection);

			AssetDatabase.RemoveObjectFromAsset(transition);
			AssetDatabase.SaveAssets();
		}
#endif

        public void ClearConnections(Node parent)
        {
            BehaviourNode actionNode = parent as BehaviourNode;
            if (actionNode != null)
            {
                actionNode.connections.Clear();
            }

            EnterNode enterNode = parent as EnterNode;
            if (enterNode != null)
            {
                enterNode.connection = null;
            }
        }

        public List<Node> GetConnections(Node parent)
        {
            List<Node> connections = new List<Node>();
            BehaviourNode actionNode = parent as BehaviourNode;
            if (actionNode != null)
            {
                return actionNode.connections;
            }

            EnterNode enterNode = parent as EnterNode;
            if (enterNode != null && enterNode.connection != null)
            {
                connections.Add(enterNode.connection);
            }
            return connections;
        }

        public List<Transition> GetTransitions(Node parent)
        {
            List<Transition> transitions = new List<Transition>();
            if (parent is BehaviourNode tacticNode)
            {
                return tacticNode.transitions;
            }

            EnterNode enterNode = parent as EnterNode;
            if (enterNode != null && enterNode.transition != null)
            {
                transitions.Add(enterNode.transition);
            }
            return transitions;
        }

        public void AddNode(Node node)
        {
            nodes.Add(node);
        }

        public TacticGraph Clone()
        {
            TacticGraph graph = Instantiate(this);
            graph.nodes = new List<Node>();
            var transitionsGUID = new Dictionary<string, List<string>>();
            for (int i = 0; i < nodes.Count; i++)
			{
                var transitions = GetTransitions(nodes[i]);
                var guids = new List<string>();
				for (int j = 0; j < transitions.Count; j++)
                {
                    guids.Add(transitions[j].connection.guid);
                }
                transitionsGUID.Add(nodes[i].guid, guids);
                var nodeInst = nodes[i].Clone();
                graph.nodes.Add(nodeInst);
                if (nodes[i] is EnterNode)
                {
                    graph.rootNode = nodeInst;
                }
			}
			for (int i = 0; i < graph.nodes.Count; i++)
			{
                var connections = transitionsGUID[graph.nodes[i].guid];
                var transitions = GetTransitions(graph.nodes[i]);
                for (int j = 0; j < connections.Count; j++)
				{
                    var connection = graph.nodes.Find(x => x.guid == connections[j]);
                    transitions[j].parent = graph.nodes[i];
                    transitions[j].connection = connection;
                    transitions[j].graph = graph;
                }
			}
            return graph;
        }
	}
}