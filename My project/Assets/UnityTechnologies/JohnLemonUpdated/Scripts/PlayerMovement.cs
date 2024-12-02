using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public InputAction MoveAction;

    public float turnSpeed = 20f;
    public float normalSpeed = 1f; 
    public float boostedSpeed = 2f; 
    public float boostDuration = 2f; 
    public float boostCooldown = 3f; 

    Animator m_Animator;
    Rigidbody m_Rigidbody;
    AudioSource m_AudioSource;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;

    private float currentSpeed;
    private bool isBoosting = false;
    private bool boostOnCooldown = false;

    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_AudioSource = GetComponent<AudioSource>();

        MoveAction.Enable();
        currentSpeed = normalSpeed;
    }

    void FixedUpdate()
    {
        var pos = MoveAction.ReadValue<Vector2>();

        float horizontal = pos.x;
        float vertical = pos.y;

        
        if (Keyboard.current.leftShiftKey.isPressed && !isBoosting && !boostOnCooldown)
        {
            StartCoroutine(BoostSpeed());
        }

        m_Movement.Set(horizontal, 0f, vertical);
        m_Movement.Normalize();

        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);
        bool isWalking = hasHorizontalInput || hasVerticalInput;
        m_Animator.SetBool("IsWalking", isWalking);

        if (isWalking)
        {
            if (!m_AudioSource.isPlaying)
            {
                m_AudioSource.Play();
            }
        }
        else
        {
            m_AudioSource.Stop();
        }

        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
        m_Rotation = Quaternion.LookRotation(desiredForward);
    }

    void OnAnimatorMove()
    {
        m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * m_Animator.deltaPosition.magnitude * currentSpeed);
        m_Rigidbody.MoveRotation(m_Rotation);
    }

    private IEnumerator BoostSpeed()
    {
        isBoosting = true;
        currentSpeed = boostedSpeed;

        yield return new WaitForSeconds(boostDuration);

        currentSpeed = normalSpeed;
        isBoosting = false;
        boostOnCooldown = true;

        
        yield return new WaitForSeconds(boostCooldown);
        boostOnCooldown = false;
    }
}