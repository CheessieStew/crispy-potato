using System;
using UnityEngine;

namespace VillagerAction
{
	public class GoInside : Action
	{
		private GameObject target;
		private GameObject subject;

		public GoInside(GameObject target)
		{
			this.target = target;
		}

		override public GameObject Subject {
			get {
				return subject;
			}

			set {
				subject = value;
			}
		}

		override public bool Finished {
			get {
				return subject.GetComponent<VillagerScript>().Container == target;
			}
		}

		override public bool Finishable {
			get{ return true; }
		}

		override public void Update()
		{
			var component = subject.GetComponent<VillagerScript>();
			if (component.CanGoInside(target))
				component.Container = target;
		}
	}
}

