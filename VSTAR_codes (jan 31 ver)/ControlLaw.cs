public class ControlLaw : MonoBehaviour
{
    [Header("RCS Parameters")]
    [SerializeField] private float thrust = 216f;
    
    [SerializeField] private float MIB = 0.028f;

    private Vector3 rotImpulseAccumulator = Vector3.zero;
    private Vector3 posImpulseAccumulator = Vector3.zero;

    public Vector3 Tb_ext { get; private set; }
    public Vector3 F_ext  { get; private set; }

    void FixedUpdate()
    {
        //thrusters OFF at start of loop
        Tb_ext = Vector3.zero;
        F_ext  = Vector3.zero;

        // --- ROTATION ---
        Vector3 torqueCmd = RotationalController.torqueCmd;
        rotImpulseAccumulator += torqueCmd * Time.fixedDeltaTime;

        for (int i = 0; i < 3; i++)
        {
            if (Mathf.Abs(rotImpulseAccumulator[i]) >= MIB)
            {
                Tb_ext[i] = Mathf.Sign(rotImpulseAccumulator[i]) * thrust * 5;
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
                F_ext[i] = Mathf.Sign(posImpulseAccumulator[i]) * thrust;
                posImpulseAccumulator[i] = 0f;
            }
        }
    }
}
