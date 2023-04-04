using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RaptorijDevelop.BehaviourGraphs
{
	public class EnterNode : Node
	{
		public Node connection;

		public Transition transition;

		protected override void Initialize()
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
				EnterNode node = Instantiate(this);
				node.instatiated = true;
				node.transition = Instantiate(transition);
				return node;
			}
		}
	}
}