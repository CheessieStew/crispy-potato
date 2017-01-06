using System;
using UnityEngine;

namespace VillagerAction
{
	public class GoOutside : Action
	{
		private GameObject subject;
		private Vector3 direction;

		public GoOutside(Vector3 direction)
		{
			this.direction = direction;
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
				return subject.GetComponent<VillagerScript>().Container == null;
			}
		}

		override public bool Finishable {
			get{ return true; }
		}

		override public void Update()
		{
			var component = subject.GetComponent<VillagerScript>();
			if (component.Container != null)
			{
				var pos = component.Container.transform.position + direction.normalized * 10;
				pos.y += 2;
				component.Container = null;
				component.transform.position = pos;
			}
		}
	}
}

