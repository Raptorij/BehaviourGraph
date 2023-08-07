using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RaptorijDevelop.BehaviourGraph
{
    [CreateAssetMenu(menuName = "Tactic Graph/Prefered Types", fileName = "PreferedTypes")]
	public class PreferedTypes : ScriptableObject
	{
		public List<string> types = new List<string>();
	}
}