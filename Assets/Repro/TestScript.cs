using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
	public Animator animator;
	public Transform IKTarget, itemToGrab;

    private void Start()
	{
		IKTarget.position = itemToGrab.position;  
	}
}
