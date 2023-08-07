#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace RaptorijDevelop.BehaviourGraph
{
    public abstract class Condition : ScriptableObject
    {
#if UNITY_EDITOR
        [ContextMenu("Remove")]
        void Remove()
        {
            AssetDatabase.RemoveObjectFromAsset(this);
            AssetDatabase.SaveAssets();
            DestroyImmediate(this);
        }
#endif

        private Transition _transition;
		protected Transition Transition => _transition;
        public event System.Action<Condition> TransitionChecked;

        public void Initialize(Transition transition)
        {
            _transition = transition;
            Initialize();
        }

        protected virtual void Initialize()
        {
            
        }

        public abstract bool CheckCondition();

        protected void Pass()
        {
            TransitionChecked?.Invoke(this);
        }

        public virtual void OnParentNodeActivated()
        {
            
        }

        public virtual void OnParentNodeDeactivated()
        {
            
        }
    }
}