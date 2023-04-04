using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace RaptorijDevelop.BehaviourGraphs
{
	public class BlackboardSource
	{
		[SerializeField] private Dictionary<string, Variable> _variables = new Dictionary<string, Variable>(StringComparer.Ordinal);

		public Dictionary<string, Variable> variables { get { return _variables; } set { _variables = value; } }
		public BlackboardSource() { }
	}
}