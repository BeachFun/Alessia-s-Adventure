using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]

public class Heroine : MonoBehaviour
{
    private enum States { Movement, Attack, Jump }

    [Header("Characteristics")]
    [SerializeField] private int _hp = 5;
    [SerializeField] private int _atk = 3;
    [SerializeField] private int _def = 1;

    [SerializeField] private float _moveSpeed = 0f;

    [Header("Ground Check")]
    [SerializeField] private bool _isGrounded;
    [SerializeField] private float _checkRadius;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private Vector3 _checkerOffset;


    [Header("Heroine class Components")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animator _animator;
    [SerializeField] private Rigidbody2D _physic;


    private void FixedUpdate()
    {
        
    }
}
