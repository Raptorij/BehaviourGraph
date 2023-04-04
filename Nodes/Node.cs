using System.Collections;
using System.Collections.Generic;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace RaptorijDevelop.BehaviourGraphs
{
	public class Node : ScriptableObject
	{
		public enum State
		{
			Running,
			Failure,
			Success
		}

		[HideInInspector]
		public State state = State.Running;
		[HideInInspector]
		public bool started = false;
		[HideInInspector]
		public string guid;
		[HideInInspector]
		public Vector2 position;
		[HideInInspector]
		public BehaviourGraphBase graph;

		//[SerializeField]
		[HideInInspector]
		protected bool instatiated;
		public bool isInstatiated => instatiated;

		public System.Action<Node> NodeUpdated;

		public void Initialize(BehaviourGraphBase graph)
		{
			this.graph = graph;
			if (this is IActionNode actionNode)
			{
				actionNode.Initialize();
			}
			Initialize();
		}

		protected virtual void Initialize()
		{
		}

		public State Update()
		{
			UpdateActionNode();
			return state;
		}

		private void UpdateActionNode()
		{
			if (this is IActionNode actionNode)
			{
				if (!started)
				{
					actionNode.OnStart();
					state = State.Running;
					started = true;
				}

				actionNode.OnUpdate();

				if (state == State.Failure || state == State.Success)
				{
					actionNode.OnStop();
					started = false;
				}

			}
		}

		public virtual Node Clone()
		{
			if (instatiated)
			{
				return this;
			}
			else
			{
				Node node = Instantiate(this);
				node.instatiated = true;
				return node;
			}
		}
	}

#if UNITY_EDITOR
	//[CustomEditor(typeof(Node))]
	//public class NodeEditor : Editor
	//{
	//	private GameObject gameObject;
	//	private Editor gameObjectEditor;

	//	private float currentTime;

	//	public override void OnInspectorGUI()
	//	{
	//		Node node = target as Node;
	//		EditorGUI.BeginChangeCheck();
	//		base.OnInspectorGUI();
	//		if (EditorGUI.EndChangeCheck())
	//		{
	//			var transitions = node.graph.GetTransitions(node);
	//			for (int i = 0; i < transitions.Count; i++)
	//			{
	//				transitions[i].name = $"{node.name}=>{transitions[i].connection.name}";
	//			}
	//			node.NodeUpdated?.Invoke(node);
	//		}

	//		GUI.enabled = false;
	//		EditorGUILayout.TextField("GUID", node.guid);
	//		GUI.enabled = true;
	//	}
	//}
#endif
}