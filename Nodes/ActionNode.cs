using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RaptorijDevelop.BehaviourGraph
{
	public class ActionNode : Node, IActionNode
	{
		void IActionNode.Initialize()
		{

		}

		void IActionNode.OnStart()
		{

		}

		void IActionNode.OnStop()
		{

		}

		void IActionNode.OnUpdate()
		{

		}
	}

	public interface IActionNode
	{
		void Initialize();
		void OnStart();
		void OnStop();
		void OnUpdate();
	}
}