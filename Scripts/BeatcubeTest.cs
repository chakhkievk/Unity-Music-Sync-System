using UnityEngine;

public class BeatCubeTest : MonoBehaviour
{
    [Header("Настройки пульсации")]
    public float pulseScale = 1.5f;   
    public float pulseSpeed = 5f;     
    private Vector3 initialScale;

    private bool pulsing = false;     
    private bool scalingUp = true;    

    void Start()
    {
        initialScale = transform.localScale;
    }

    void Update()
    {
        if (pulsing)
        {
            float step = pulseSpeed * Time.deltaTime;
            Vector3 target = scalingUp ? initialScale * pulseScale : initialScale;
            transform.localScale = Vector3.MoveTowards(transform.localScale, target, step);

            if (Vector3.Distance(transform.localScale, target) < 0.001f)
            {
                if (scalingUp)
                    scalingUp = false; 
                else
                    pulsing = false;  
            }
        }
    }

    
    public void Pulse()
    {
        if (!pulsing)
        {
            pulsing = true;
            scalingUp = true;
        }
    }
}
