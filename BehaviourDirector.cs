using UnityEngine;
using CerealDevelopment.TimeManagement;
using CerealDevelopment.LifetimeManagement;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RaptorijDevelop.BehaviourGraphs
{
	public class BehaviourDirector : LifetimeMonoBehaviour,
		IUpdatable
	{
		[SerializeField]
		private BehaviourGraphBase behaviourGraph;
		public BehaviourGraphBase BehaviourGraph => behaviourGraph;

		protected override void Initialize()
		{
			base.Initialize();
			behaviourGraph = behaviourGraph.Clone();
			behaviourGraph.Initialize(this);
			this.EnableUpdates();
		}

		protected override void Dispose()
		{
			base.Dispose();
			this.DisableUpdates();
		}

		void IUpdatable.OnUpdate()
		{
			behaviourGraph.Update();
		}
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(BehaviourDirector))]
	public class TacticDirectorEditor : Editor
	{
		private static readonly string[] _dontIncludeMe = new string[] { "m_Script" };

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			DrawPropertiesExcluding(serializedObject, _dontIncludeMe);

			serializedObject.ApplyModifiedProperties();
		}
	}
#endif
}