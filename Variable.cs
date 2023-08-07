using System;
using UnityEngine;

namespace RaptorijDevelop.BehaviourGraph
{
	public class Variable
	{
		public string name;
		public object value 
		{
			get 
			{
				return GetValueBoxed(); 
			} 
			set 
			{ 
				SetValueBoxed(value); 
			} 
		}
		virtual public Type varType { get; }
		public event Action<object> onValueChanged;
		public virtual object GetValueBoxed() { return null; }
		public virtual void SetValueBoxed(object value) { }
		protected bool HasValueChangeEvent() 
		{ 
			return onValueChanged != null; 
		}
	}

	[Serializable]
	public class Variable<T> : Variable
	{
		[SerializeField] private T _value;

		new public T value
		{
			get { return _value; }
			set
			{
				this._value = value;
			}
		}

		public override Type varType => typeof(T);

		public override object GetValueBoxed()
		{
			return value;
		}
		public override void SetValueBoxed(object newValue)
		{
			this.value = (T)newValue;
		}
	}
}