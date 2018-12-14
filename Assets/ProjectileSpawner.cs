using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ECS.Statics;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour 
{
	void Update () 
	{
		if (Input.GetMouseButton(0))
		{
			for (int i = 0; i < 1; i++)
				EcsBootstrap.CreateProjectile(transform.position, transform.forward * 5, 10);
		}
	}
}