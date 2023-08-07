using UnityEngine;
using CerealDevelopment.TimeManagement;
using CerealDevelopment.LifetimeManagement;
using CerealDevelopment.UnitFramework;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RaptorijDevelop.BehaviourGraph
{
	public class TacticDirector : LifetimeMonoBehaviour,
		IUpdatable
	{
		public IUnitBase Character { get; private set; }

		[SerializeField]
		private TacticGraph animationGraph;
		public TacticGraph AnimationGraph => animationGraph;

		protected override void Construct()
		{
			base.Construct();
			animationGraph = animationGraph.Clone();
			Character = GetComponentInParent<IUnitBase>();
			ActivateDirector(Character);
			animationGraph.Initialize(this);
			Character.Activated += ActivateDirector;
			Character.Deactivated += DeactivateDirector;
		}

		private void ActivateDirector(IUnitBase obj)
		{
			animationGraph.RemoveDeactivator(this);
			this.EnableUpdates();
		}

		private void DeactivateDirector(IUnitBase obj)
		{
			this.DisableUpdates();
			animationGraph.AddDeactivator(this);
		}

		protected override void Initialize()
		{
			base.Initialize();
		}

		protected override void Dispose()
		{
			base.Dispose();
			Character.Activated -= ActivateDirector;
			Character.Deactivated -= DeactivateDirector;
			this.DisableUpdates();
		}

		void IUpdatable.OnUpdate()
		{
			animationGraph.Update();
		}
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(TacticDirector))]
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