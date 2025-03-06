using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject ObstaclePrefab;
    public float SpawnInterval = 1.5f;
    public float Speed = 5f;
    public float Despawn = -10f;
    private List<GameObject> Obstacles = new List<GameObject>();

    private void Start()
    {
        InvokeRepeating(nameof(SpawnObstacle), 1f, SpawnInterval);
    }

    private void Update()
    {
        // Loop through the obstacles list in reverse to safely remove destroyed objects
        for (int a = Obstacles.Count - 1; a >= 0; a--)
        {
            if (Obstacles[a] == null)
            {
                Obstacles.RemoveAt(a);  // Remove destroyed obstacles from the list
            }
        }

        MoveObstacles();
        CheckCollisions();
    }

    private void SpawnObstacle()
    {
        List<int> AvailableLanes = new List<int> { -2, 0, 2 }; // X positions of the lanes

        // Randomly decide if 1 or 2 lanes will be open
        int OpenLanes = Random.Range(1, 3);

        // Randomly select lanes to keep open
        for (int a = 0; a < OpenLanes; a++)
        {
            int IndexToRemove = Random.Range(0, AvailableLanes.Count);
            AvailableLanes.RemoveAt(IndexToRemove);
        }

        // Spawn obstacles in the remaining lanes with random height
        foreach (int LaneX in AvailableLanes)
        {
            Vector3 SpawnPosition = new Vector3(LaneX, 0, 20f);
            GameObject obstacle = Instantiate(ObstaclePrefab, SpawnPosition, Quaternion.identity);

            // Set a random height (between 1 and 3 units tall)
            float randomHeight = Random.Range(1f, 3f);
            obstacle.transform.localScale = new Vector3(1, randomHeight, 1);

            Obstacles.Add(obstacle);
        }
    }

    private float GetRandomLaneX()
    {
        int Lane = Random.Range(0, 3);
        return Lane * 2f - 2f;
    }

    private void MoveObstacles()
    {
        for (int a = Obstacles.Count - 1; a >= 0; a--)
        {
            GameObject Obs = Obstacles[a];

            // Move the obstacle towards the player
            Obs.transform.position -= new Vector3(0, 0, Speed * Time.deltaTime);

            // Destroy obstacles when they go out of bounds
            if (Obs.transform.position.z < Despawn)
            {
                Destroy(Obs);
                Obstacles.RemoveAt(a);
            }
        }
    }

    private void CheckCollisions()
    {
        GameObject Player = GameObject.FindWithTag("Player");
        if (Player == null) return;

        PlayerController PlayerController = Player.GetComponent<PlayerController>();
        Vector3 PlayerPos = Player.transform.position;

        for (int a = Obstacles.Count - 1; a >= 0; a--)
        {
            GameObject Obs = Obstacles[a];
            Vector3 ObsPos = Obs.transform.position;

            if (Mathf.Abs(PlayerPos.z - ObsPos.z) < 1f && Mathf.Abs(PlayerPos.x - ObsPos.x) < 1f && PlayerPos.y < 1f)
            {
                PlayerController.TakeDamage();
                Destroy(Obs);
                Obstacles.RemoveAt(a);
            }
        }
    }
}
