using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RaptorijDevelop.BehaviourGraphs
{
    public abstract class NodeAbility : ScriptableObject
    {
        [ContextMenu("Remove")]
        void Remove()
        {
            AssetDatabase.RemoveObjectFromAsset(this);
            AssetDatabase.SaveAssets();
            DestroyImmediate(this);
        }

        public void OnDestroy()
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