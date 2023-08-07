#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace RaptorijDevelop.BehaviourGraph
{
    public class NodeAbility : ScriptableObject
    {
        protected BehaviourNode behaviourNode;

#if UNITY_EDITOR
        [ContextMenu("Remove")]
        void Remove()
        {
            AssetDatabase.RemoveObjectFromAsset(this);
            AssetDatabase.SaveAssets();
            DestroyImmediate(this);
        }
#endif

        public void OnDestroy()
        {

        }

        public void Initialize(BehaviourNode behaviourNode)
        {
            this.behaviourNode = behaviourNode;
            Initialize();
        }

        protected virtual void Initialize()
        {
            
        }

        public virtual void OnEnter(BehaviourNode behaviourNode)
        {
            
        }

        public virtual void OnUpdate(BehaviourNode behaviourNode)
        {

        }

        public virtual void OnExit(BehaviourNode behaviourNode)
        {

        }
    }
}