using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyYRotation : MonoBehaviour
{
	[SerializeField] private Transform target;
	[SerializeField] private float smoothSpeed = 5;

	private void Start()
	{
		transform.position = target.position;
	}

	private void Update()
	{
		Quaternion targetRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, target.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smoothSpeed);
	}
}
