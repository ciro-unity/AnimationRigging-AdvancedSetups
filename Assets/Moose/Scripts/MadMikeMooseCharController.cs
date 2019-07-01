using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;

public class MadMikeMooseCharController : MonoBehaviour
{
	public float speedMultiplier = 4f;
	public Transform gazeVirtualBone;
	public MultiAimConstraint headMultiAim;
	public GameObject selectionMarker;
	
	private float _currentSpeed;
	private Animator _animator;
	private Transform _transform;
	private GameObject[] _interactableObjects;
	private Plane _forwardPlane;
	private float _gazeMaxSqrDistance = 4f;
	private GameObject _closest;
	private bool _wasLookingAtSomething = false;

	public UnityAction GazeConnected, GazeDisconnected;

    void Awake()
    {
        _animator = GetComponent<Animator>();
		_transform = GetComponent<Transform>();
		_forwardPlane = new Plane();
		selectionMarker.SetActive(false);
    }

	void Start()
	{
		_interactableObjects = GameObject.FindGameObjectsWithTag("Interactable");
	}

    void Update()
    {
		//Input, Movement and Animation routines
		float _hor = Input.GetAxis("Horizontal");
		float _ver = Input.GetAxis("Vertical");
		Vector3 _inputVector = (new Vector3(_hor, 0f, _ver)).normalized;
		float _input = _inputVector.magnitude;

        if(_inputVector.sqrMagnitude != 0f)
		{
			Vector3 newPosition = _transform.position + _inputVector * Time.deltaTime * speedMultiplier;
			
			NavMeshHit hit;
			NavMesh.SamplePosition(newPosition, out hit, .3f, NavMesh.AllAreas);
			bool hasMoved = (_transform.position-hit.position).magnitude >= .02f;
			
			if(hasMoved)
			{
				_transform.position = hit.position;
				_animator.SetBool("IsMoving", hasMoved);
				_transform.forward = Vector3.Lerp(_transform.forward, _inputVector, Time.deltaTime * 10f);
			}
			else
			{
				_animator.SetBool("IsMoving", false);
			}

			_animator.SetFloat("CurrentSpeed", _input);
		}
		else
		{
			_animator.SetBool("IsMoving", false);
		}

		//Orient and move the plane which limits the gaze
		_forwardPlane.SetNormalAndPosition(_transform.forward, _transform.position);

		//Head aim routine
		if(_interactableObjects.Length != 0)
		{
			_closest = null;
			float closestSqrDistance = Mathf.Infinity;
			for(int i=0; i<_interactableObjects.Length; i++)
			{
				float sqrDistance = (_transform.position - _interactableObjects[i].transform.position).sqrMagnitude;
				
				//If closer than the previous AND within gaze range AND on the right side of the plane
				if(sqrDistance < closestSqrDistance
					&& sqrDistance <= _gazeMaxSqrDistance
					&& _forwardPlane.GetSide(_interactableObjects[i].transform.position))
				{
					_closest = _interactableObjects[i];
					closestSqrDistance = sqrDistance;
				}
			}

			//Fire the event for the GameplayManager
			if(!_wasLookingAtSomething && _closest != null)
			{
				selectionMarker.SetActive(true);
				GazeConnected.Invoke();
			}
			else if(_wasLookingAtSomething && _closest == null)
			{
				selectionMarker.SetActive(false);
				GazeDisconnected.Invoke();
			}

			//Adjust the gaze constraint
			if(_closest != null)
			{
				headMultiAim.weight = Mathf.Lerp(headMultiAim.weight, 1f, Time.deltaTime * 20f);
				gazeVirtualBone.position = _closest.transform.position;
				_wasLookingAtSomething = true;
			}
			else
			{
				headMultiAim.weight = Mathf.Lerp(headMultiAim.weight, 0f, Time.deltaTime * 10f);
				_wasLookingAtSomething = false;
			}
		}
    }
}
