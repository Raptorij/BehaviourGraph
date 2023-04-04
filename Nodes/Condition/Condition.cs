using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RaptorijDevelop.BehaviourGraphs
{
    public abstract class Condition : ScriptableObject
    {
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