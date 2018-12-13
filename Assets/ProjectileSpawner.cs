using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ECS.Statics;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour 
{
	void Update () 
	{
		if (Input.GetMouseButtonDown(0))
		{
			for (int i = 0; i < 200; i++)
				EcsEntityManager.CreateProjectile(transform.position, transform.forward * 303, 10);
		}
	}
}