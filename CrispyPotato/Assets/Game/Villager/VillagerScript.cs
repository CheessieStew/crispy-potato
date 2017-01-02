using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class VillagerScript : MonoBehaviour
{
	public Side Side;

	public int Strength = 5;
	public int Agility = 5;
	public int Constitution = 5;
	public int Dexterity = 5;
	public int Speed = 5;

	private GameObject container = null;

	public GameObject Container {
		get { return container; }
		set {
			if (value == null)
			{
				if (container != null)
				{
					var tmp = container.GetComponent<VillagersContainer>();
					container = null;
					tmp.Remove(gameObject);
				}
			}
			else
			{
				if (container == null)
				{
					container = value;
					container.GetComponent<VillagersContainer>().Add(gameObject);
				}
				else
				{
					Container = null;
					Container = value;
				}
			}
		}
	}

	private static void mutate(GameObject villager)
	{
		var scr = villager.GetComponent<VillagerScript>();
		scr.Agility = System.Math.Max(0, scr.Agility + UnityEngine.Random.Range(-3, 3));
		scr.Dexterity = System.Math.Max(0, scr.Dexterity + UnityEngine.Random.Range(-3, 3));
		scr.Strength = System.Math.Max(0, scr.Strength + UnityEngine.Random.Range(-3, 3));
		scr.Constitution = System.Math.Max(0, scr.Constitution + UnityEngine.Random.Range(-3, 3));
		scr.Speed = System.Math.Max(0, scr.Speed + UnityEngine.Random.Range(-3, 3));
	}

	public static GameObject CreateRandom(GameObject prefab)
	{
		var villager = Instantiate(prefab);
		mutate(villager);
		return villager;
	}

	// Use this for initialization
	void Start()
	{
		
	}
	
	// Update is called once per frame
	void Update()
	{
		AI();
	}

	private Vector2 target = Vector2.zero;

	public void AI()
	{
		if (Container != null)
		{
			if (UnityEngine.Random.Range(0, 1000) == 0)
			{
				var dir = UnityEngine.Random.insideUnitSphere;
				dir.y = 0;
				GoOutside(dir);
			}
		}
		else
		{
			if (target == Vector2.zero || Vector2.Distance(new Vector2(transform.position.x, transform.position.z), target) < 10)
				target = new Vector2(UnityEngine.Random.Range(10, 490), UnityEngine.Random.Range(10, 490));

			GoTowards(target);
		}
	}
}
