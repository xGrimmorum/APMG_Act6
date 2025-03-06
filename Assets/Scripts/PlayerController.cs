using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float JumpHeight = 5f;
    public float LaneDistance = 2f;
    public float Gravity = 9.81f;
    private float VerticalVelocity;
    private int CurrentLane = 1; // 0 = Left, 1 = Middle, 2 = Right
    private float HP = 100f;
    public TextMeshProUGUI HPText;

    private Vector3 Position;
    private bool IsJumping = false;

    private void Start()
    {
        Position = transform.position;
        InvokeRepeating(nameof(RegenerateHP), 1f, 1f);
    }

    private void Update()
    {
        HandleMovement();
        ApplyGravity();
        UpdatePosition();
        HPText.text = "HP: " + Mathf.Ceil(HP);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            if (HP > 0)
            {
                HP -= 5;
                HPText.text = "HP: " + HP;
            }

            // Disable the obstacle first to prevent further access issues
            other.gameObject.SetActive(false);

            // Destroy the object with a slight delay
            Destroy(other.gameObject, 0.1f);
        }
    }

    private void HandleMovement()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) && CurrentLane > 0)
        {
            CurrentLane--;
            Position.x -= LaneDistance;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) && CurrentLane < 2)
        {
            CurrentLane++;
            Position.x += LaneDistance;
        }
        if (Input.GetKeyDown(KeyCode.Space) && !IsJumping)
        {
            VerticalVelocity = JumpHeight;
            IsJumping = true;
        }
    }

    private void ApplyGravity()
    {
        if (IsJumping)
        {
            VerticalVelocity -= Gravity * Time.deltaTime;
            Position.y += VerticalVelocity * Time.deltaTime;
            if (Position.y <= 0)
            {
                Position.y = 0;
                IsJumping = false;
            }
        }
    }

    private void UpdatePosition()
    {
        transform.position = Position;
    }

    public void TakeDamage()
    {
        HP = Mathf.Max(HP - 5, 0);
    }

    private void RegenerateHP()
    {
        HP = Mathf.Min(HP + 1, 100);
    }
}
