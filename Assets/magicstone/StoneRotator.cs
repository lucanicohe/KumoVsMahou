using System.Collections;
using UnityEngine;

public class StoneRotator : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 30f;

    private float originalSpeed;
    private Coroutine boostRoutine;

    void Start()
    {
        originalSpeed = rotationSpeed;
    }

    void Update()
    {
        Vector3 currentRotation = transform.localEulerAngles;
        currentRotation.y += rotationSpeed * Time.deltaTime;
        transform.localEulerAngles = currentRotation;
    }

    public void BoostRotation(float boostSpeed, float duration)
    {
        if (boostRoutine != null)
            StopCoroutine(boostRoutine);

        boostRoutine = StartCoroutine(BoostRoutine(boostSpeed, duration));
    }

    private IEnumerator BoostRoutine(float boostSpeed, float duration)
    {
        rotationSpeed = boostSpeed;
        yield return new WaitForSeconds(duration);
        rotationSpeed = originalSpeed;
        boostRoutine = null;
    }
}