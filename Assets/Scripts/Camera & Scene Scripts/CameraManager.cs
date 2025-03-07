using UnityEngine;
using System.Collections.Generic;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    public CinemachineCamera cinemachineCam;
    private List<Transform> playerTargets = new List<Transform>();
    private int currentTargetIndex = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        cinemachineCam = FindFirstObjectByType<CinemachineCamera>(); // Find the camera in the scene
        FindExistingPlayers(); // Register any already existing players
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && playerTargets.Count > 1)
        {
            SwitchTarget();
        }
    }

    public void RegisterPlayer(GameObject player)
    {
        if (!playerTargets.Contains(player.transform))
        {
            playerTargets.Add(player.transform);

            // If it's the first player, set it as the target
            if (playerTargets.Count == 1)
            {
                SetCameraTarget(player.transform);
            }
        }
    }

    private void FindExistingPlayers()
    {
        GameObject[] existingPlayers = GameObject.FindGameObjectsWithTag("Player"); // Detects all game object with "Player" tag

        foreach (GameObject player in existingPlayers)
        {
            RegisterPlayer(player);
        }
    }

    private void SwitchTarget()
    {
        currentTargetIndex = (currentTargetIndex + 1) % playerTargets.Count;
        SetCameraTarget(playerTargets[currentTargetIndex]);
    }

    private void SetCameraTarget(Transform target)
    {
        cinemachineCam.Follow = target;
        cinemachineCam.LookAt = target;
    }
}
