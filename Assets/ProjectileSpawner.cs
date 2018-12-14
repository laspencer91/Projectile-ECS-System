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
			for (int i = 0; i < 100; i++)
				EcsBootstrap.CreateProjectile(transform.position, transform.forward * 600, transform.rotation, 3, 10);
		}
	}
}