using System.Collections.Generic;
using UnityEngine;

namespace RaptorijDevelop.BehaviourGraph
{
	public class Transition : ScriptableObject
	{
		[HideInInspector]
		public TacticGraph graph;
		[HideInInspector]
		public string guid;
		[HideInInspector]
		public Node parent;
		[HideInInspector]
		public Node connection;

		[HideInInspector]
		public List<Condition> conditions = new List<Condition>();
		[HideInInspector]
		protected bool instatiated;

		public void Initialize()
		{
			for (int i = 0; i < conditions.Count; i++)
			{
				conditions[i].Initialize(this);
			}
		}

		public void OnParentNodeStart()
		{
			for (int i = 0; i < conditions.Count; i++)
			{
				conditions[i].TransitionChecked += OnConditionChanged;
				conditions[i].OnParentNodeActivated();
			}
		}

		public void OnParentNodeStop()
		{
			for (int i = 0; i < conditions.Count; i++)
			{
				conditions[i].TransitionChecked -= OnConditionChanged;
				conditions[i].OnParentNodeDeactivated();
			}
		}

		private void OnConditionChanged(Condition condition)
		{
			if (parent is BehaviourNode tacticNode)
			{
				tacticNode.TryTransitBy(this);
			}
		}

		public bool CheckTransition()
		{
			bool canTransit = true;
			for (int i = 0; i < conditions.Count; i++)
			{
				canTransit &= conditions[i].CheckCondition();
			}
			return canTransit;
		}

		public Transition Clone()
		{
			if (instatiated)
			{
				return this;
			}
			else
			{
				Transition transition = Instantiate(this);
				transition.conditions.Clear();
				for (int i = 0; i < conditions.Count; i++)
				{
					transition.conditions.Add(Instantiate(conditions[i]));
				}
				transition.instatiated = true;
				return transition;
			}
		}
	}
}