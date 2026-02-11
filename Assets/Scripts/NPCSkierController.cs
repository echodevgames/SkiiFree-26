using System.Threading;
using UnityEngine;



public class NPCSkierController : MonoBehaviour
{

    [Header("Movement")]
    public float downhillSpeed = 4f;
    public float lateralDriftMultiplier = 1f;

    [Header("Behavior")]
    public float wobbleStrength = 0.5f;
    public float wobbleSpeed = 2f;

    float wobbleOffset;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        var gm = GameManager.Instance;

        float vertical = downhillSpeed;

        float horizontal =
            gm.baseScrollSpeed *
            gm.LateralFactor *
            gm.HeadingDirection *
            lateralDriftMultiplier;

        wobbleOffset += Time.deltaTime * wobbleSpeed;
        horizontal += Mathf.Sin(wobbleOffset) * wobbleStrength;

        Vector3 move = Vector3.down * vertical * Time.deltaTime + Vector3.right * horizontal * Time.deltaTime;

        transform.position += move;



    }
}
