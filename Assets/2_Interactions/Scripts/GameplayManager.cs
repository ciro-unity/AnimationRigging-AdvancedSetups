using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameplayManager : MonoBehaviour
{
	public MadMikeMooseCharController mmmCharController;
	public CinemachineVirtualCamera normalVCam, closeupVCam;

    void Start()
    {
        mmmCharController.GazeConnected += OnGazeConnected;
        mmmCharController.GazeDisconnected += OnGazeDisconnected;
    }

    private void OnGazeConnected()
    {
        normalVCam.Priority = 0;
		closeupVCam.Priority = 100;
    }

	private void OnGazeDisconnected()
    {
		closeupVCam.Priority = 0;
        normalVCam.Priority = 100;
    }
}
