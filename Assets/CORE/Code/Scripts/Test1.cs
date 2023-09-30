using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test1 : MonoBehaviour
{
    [SerializeField] private CharacterController character;

    void Start()
    {
        character.Move(Vector3.left);
    }

}
