using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


// There are functions that should be exported to AI.
public partial class VillagerScript
{
	public bool IsInside()
	{
		return Container != null;
	}

	public bool IsBusy()
	{
		return currentAction != null;
	}

	public bool CanGoInside(GameObject container)
	{
		return Vector3.Distance(transform.position, container.transform.position) < 10 &&
		container.GetComponent<VillagersContainer>() != null;
	}

	public void GoInside(GameObject container)
	{
		if (CanGoInside(container))
			currentAction = new VillagerAction.GoInside(container);
	}

	public void GoOutside(Vector3 direction)
	{
		currentAction = new VillagerAction.GoOutside(direction);
	}

	public void GoTowards(Vector2 target)
	{
		currentAction = new VillagerAction.GoTowards(target);
	}
}
