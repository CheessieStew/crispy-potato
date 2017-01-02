using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageScript : MonoBehaviour
{
	public Side Side;
	public GameObject VillagerPrefab;

	public ushort Houses = 10;
	public ushort Warehouses = 1;
	public ushort Forges = 0;



	// Use this for initialization
	void Start()
	{
		var container = GetComponent<VillagersContainer>();
		for (uint i = 0; i < 10; i++)
			container.Add(VillagerScript.CreateRandom(VillagerPrefab));
	}
	
	// Update is called once per frame
	void Update()
	{
	}
}
