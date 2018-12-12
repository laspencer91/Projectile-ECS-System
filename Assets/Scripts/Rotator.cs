using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class Rotator : MonoBehaviour
{
	public float speed;
}

class RotatorSystem : ComponentSystem
{
	struct Components
	{
		public Rotator rotator;
		public Transform transform;
	}

	protected override void OnUpdate()
	{
		float timeStep = Time.deltaTime;
		
		foreach (var entity in GetEntities<Components>())
		{
			entity.transform.Rotate(0f, entity.rotator.speed * timeStep, 0f);
		}
	}
}
