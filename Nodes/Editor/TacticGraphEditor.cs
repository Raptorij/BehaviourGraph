using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RaptorijDevelop.BehaviourGraphs
{
    [CustomEditor(typeof(BehaviourGraphBase))]
	public class TacticGraphEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			var graph = target as BehaviourGraphBase;
			EditorGUILayout.RectField(graph.rect);
			EditorGUI.indentLevel++;
			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			EditorGUILayout.EndVertical();
			if (GUILayout.Button("Remove Note Nodes"))
			{
				for (int i = 0; i < graph.nodes.Count; i++)
				{
					if (graph.nodes[i] is NoteNode)
					{
						graph.DeleteNode(graph.nodes[i]);
						i--;
					}
				}
			}
		}
	}
};