using System;
using UnityEngine;

namespace VillagerAction
{
	public class GoTowards : Action
	{
		private Vector2 target;
		private GameObject subject;

		public GoTowards(Vector2 target)
		{
			this.target = target;
		}

		override public GameObject Subject {
			get {
				return subject;
			}

			set {
				if (subject != null)
					subject.GetComponent<Animator>().SetBool("walk", false);
				
				subject = value;

				if (subject != null)
					subject.GetComponent<Animator>().SetBool("walk", true);
			}
		}

		override public bool Finished {
			get {
				return Vector2.Distance(new Vector2(subject.transform.position.x, subject.transform.position.z), target) < 3;
			}
		}

		override public bool Finishable {
			get{ return true; }
		}

		override public void Update()
		{
			var pos2D = new Vector2(subject.transform.position.x, subject.transform.position.z);
			var diff2D = target - pos2D;
			var clamped = Vector2.ClampMagnitude(diff2D, subject.GetComponent<VillagerScript>().Speed / 50.0f);

			var target3D = new Vector3(subject.transform.position.x + clamped.x, 0, subject.transform.position.z + clamped.y);
			target3D.y = Terrain.activeTerrain.SampleHeight(target3D);
			subject.transform.position = target3D;

			var v1 = diff2D;
			var v2 = Vector2.up;
			var dot = v1.x * v2.x + v1.y * v2.y;
			var	det = v1.x * v2.y - v1.y * v2.x;
			var angle = Mathf.Atan2(det, dot) * Mathf.Rad2Deg;

			subject.transform.rotation = Quaternion.identity;
			subject.transform.Rotate(0, angle, 0);
		}
	}
}

