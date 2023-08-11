using System.Collections;
using UnityEngine;

public class HeroineController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(Loop());
    }

    private IEnumerator Loop()
    {
        yield return null;

    }
}
