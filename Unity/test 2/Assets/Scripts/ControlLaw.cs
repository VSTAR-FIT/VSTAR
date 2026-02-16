 using System.Runtime.Serialization; using System.Xml.Serialization; using UnityEngine;

public class ControlLaw : MonoBehaviour
{
    [Header("RCS Parameters")]
    [SerializeField] private float thrusterForce = 216f;
    [SerializeField] private float leverArm = 5f;
    [SerializeField] private float MIB = 0.028f;

    private Vector3 rotImpulseAccumulator = Vector3.zero;
    private Vector3 posImpulseAccumulator = Vector3.zero;

    public Vector3 Tb_ext { get; private set; }
    public Vector3 F_ext  { get; private set; }

    void FixedUpdate()
    {
        Tb_ext = Vector3.zero;
        F_ext  = Vector3.zero;

        // --- ROTATION ---
        Vector3 torqueCmd = RotationalController.rateCmd;
        rotImpulseAccumulator += torqueCmd * Time.fixedDeltaTime;

        for (int i = 0; i < 3; i++)
        {
            if (Mathf.Abs(rotImpulseAccumulator[i]) >= MIB)
            {
                Tb_ext[i] = Mathf.Sign(rotImpulseAccumulator[i]) * thrusterForce * leverArm;
                rotImpulseAccumulator[i] = 0f;
            }
        }

        // --- TRANSLATION ---
        Vector3 forceCmd = TranslationalController.forceCmd;
        posImpulseAccumulator += forceCmd * Time.fixedDeltaTime;

        for (int i = 0; i < 3; i++)
        {
            if (Mathf.Abs(posImpulseAccumulator[i]) >= MIB)
            {
                F_ext[i] = Mathf.Sign(posImpulseAccumulator[i]) * thrusterForce;
                posImpulseAccumulator[i] = 0f;
            }
        }
    }
}
