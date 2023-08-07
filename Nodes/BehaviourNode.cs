using System.Collections.Generic;
using UnityEngine;

namespace RaptorijDevelop.BehaviourGraph
{
    public abstract class BehaviourNode : ActionNode, IActionNode
	{
		public string nodeName = "New Tactic Node";
		[HideInInspector]
		public List<Node> connections = new List<Node>();
		[HideInInspector]
		public List<string> connectionsGUID = new List<string>();
		[HideInInspector]
		public List<Transition> transitions = new List<Transition>();

		[HideInInspector]
		public List<NodeAbility> abilities = new List<NodeAbility>();

		void IActionNode.Initialize()
		{
			for (int i = 0; i < transitions.Count; i++)
			{
				transitions[i].Initialize();
			}
			for (int i = 0; i < abilities.Count; i++)
			{
				abilities[i].Initialize(this);
			}
		}

		protected virtual void OnActivated() { }
		protected virtual void OnDeactivated() { }
		protected virtual new void Update() { }

		void IActionNode.OnStart()
		{
			OnActivated();
			for (int i = 0; i < abilities.Count; i++)
			{
				abilities[i].OnEnter(this);
			}
            for (int i = 0; i < transitions.Count; i++)
            {
				transitions[i].OnParentNodeStart();
            }
		}

		void IActionNode.OnStop()
		{
			OnDeactivated();
			for (int i = 0; i < abilities.Count; i++)
			{
				abilities[i].OnExit(this);
			}
			for (int i = 0; i < transitions.Count; i++)
			{
				transitions[i].OnParentNodeStop();
			}
		}

		void IActionNode.OnUpdate()
		{
			Update();

			for (int i = 0; i < abilities.Count; i++)
			{
				abilities[i].OnUpdate(this);
			}

			for (int i = 0; i < transitions.Count; i++)
			{
				if (transitions[i].CheckTransition())
				{
					TransitBy(transitions[i]);
					break;
				}
			}
		}

		public void TryTransitBy(Transition transition)
		{
			if (transition.CheckTransition())
			{
				TransitBy(transition);
			}
		}

		private void TransitBy(Transition transition)
		{
			state = State.Success;
			graph.SetCurrentNode(transition.connection);
		}

		public override Node Clone()
		{
			if (instatiated)
			{
				return this;
			}
			else
			{
				BehaviourNode node = Instantiate(this);
				node.abilities.Clear();
				for (int i = 0; i < abilities.Count; i++)
				{
					node.abilities.Add(Instantiate(abilities[i]));
				}
				node.instatiated = true;
				node.transitions.Clear();
				for (int i = 0; i < transitions.Count; i++)
				{
					node.transitions.Add(transitions[i].Clone());
				}
				return node;
			}
		}
	}
}