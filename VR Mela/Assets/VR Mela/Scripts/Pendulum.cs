using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pendulum : MonoBehaviour
{
    public float MaxAngleDeflection = 30.0f;
    public float SpeedOfPendulum = 1.0f;

    private void Start()
    {
        System.Random random = new System.Random();
        MaxAngleDeflection = MaxAngleDeflection * random.Next(1, 2);
    }

    // Update is called once per frame
    void Update()
    {
        float angle = MaxAngleDeflection * Mathf.Sin(Time.time * SpeedOfPendulum);
        gameObject.transform.localRotation = Quaternion.Euler(0, 0, angle);

    }
}
