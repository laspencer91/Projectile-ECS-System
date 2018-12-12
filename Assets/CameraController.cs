using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	Vector2 rotation = new Vector2 (0, 0);
	public float speed = 3;

	private void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
	}

	void Update () {
		rotation.y += Input.GetAxis ("Mouse X");
		rotation.x += -Input.GetAxis ("Mouse Y");
		transform.eulerAngles = (Vector2)rotation * speed;
	}
}

