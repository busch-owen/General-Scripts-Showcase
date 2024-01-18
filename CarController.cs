using UnityEngine;
public enum DrivetrainState
{
    AWD, FWD, RWD
}

public class CarController : MonoBehaviour
{
    //VEHICLE MOVEMENT AND CONTROLS IN HERE ONLY!!!!\\

    #region Variables

    private const string hori = "Horizontal";
    private const string verti = "Vertical";

    private float horizontalInput;
    private float verticalInput;
    private float steerAngle;
    private float currentBrakeForce;

    private bool isBraking;

    [Header("Forces")]

    [SerializeField] private float motorForce;
    [SerializeField] private float brakeForce;
    [SerializeField] private float maxSteerAngle;
    [SerializeField] private float downForce;
    [SerializeField] private float maxSpeed;

    [SerializeField] Vector3 centerOfMassPosition;

    [Header("Wheels")]

    [SerializeField] private WheelCollider[] wheelColliders;

    [SerializeField] private WheelCollider fl;
    [SerializeField] private WheelCollider fr;
    [SerializeField] private WheelCollider rl;
    [SerializeField] private WheelCollider rr;

    [SerializeField] private Transform flt;
    [SerializeField] private Transform frt;
    [SerializeField] private Transform rlt;
    [SerializeField] private Transform rrt;

    [Header("Suspension")]

    [SerializeField] private float suspensionSpringRate;
    [SerializeField] private float suspensionSpringDamper;
    [SerializeField] private float suspensionTargetPosition;

    [Header("Tire Compounds")]

    [SerializeField] private float forwardFrictionExtSlip;
    [SerializeField] private float forwardFrictionExtValue;
    [SerializeField] private float forwardFrictionAsymSlip;
    [SerializeField] private float forwardFrictionAsymValue;
    [SerializeField] private float forwardStiffness;

    [SerializeField] private float sidewaysFrictionExtSlip;
    [SerializeField] private float sidewaysFrictionExtValue;
    [SerializeField] private float sidewaysFrictionAsymSlip;
    [SerializeField] private float sidewaysFrictionAsymValue;
    [SerializeField] private float sidewaysStiffness;

    public DrivetrainState drivetrainState;

    [SerializeField] ParticleSystem exhaustEffects;

    Quaternion defaultRotation;

    private Rigidbody rb;

    public bool flippingCar;

    Warehouse warehouse;

    VehicleStats vehicleStats;

    WheelFrictionCurve forwardFrictionCurve;
    WheelFrictionCurve sidewaysFrictionCurve;
    JointSpring suspensionSpring;

    #endregion

    #region Unity Runtime Functions

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        warehouse = FindObjectOfType<Warehouse>();
        vehicleStats = FindObjectOfType<VehicleStats>();
        defaultRotation = transform.rotation;
        rb.centerOfMass = centerOfMassPosition;
    }

    private void Update()
    {
        vehicleStats = FindObjectOfType<VehicleStats>();
        UpdateStats();
        AssignValuesToObjects();
        if (warehouse?.inMenu == true)
        {
            rb.velocity = Vector3.zero;
            return;
        }
        GetInput();
    }

    private void FixedUpdate()
    {
        if (warehouse?.inMenu == true)
            return;

        HandleMotor();
        HandleSteering();
        UpdateWheels();

        if (transform.rotation.x is < 0.01f and > -0.01f && transform.rotation.z is < 0.01f and > -0.01f)
            flippingCar = false;

        if (flippingCar)
            FlipCar();

        if(rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }

        rb.AddForce(downForce * Mathf.Abs(rb.velocity.x) * Vector3.up);
    }

    #endregion

    #region Input and Physics

    private void GetInput()
    {
        horizontalInput = Input.GetAxis(hori);
        verticalInput = Input.GetAxis(verti);
        isBraking = Input.GetButton("Brake");
    }

    private void HandleMotor()
    {
        switch (drivetrainState)
        {
            case DrivetrainState.AWD:
                fl.motorTorque = verticalInput * motorForce;
                fr.motorTorque = verticalInput * motorForce;
                rl.motorTorque = verticalInput * motorForce;
                rr.motorTorque = verticalInput * motorForce;
                break;
            case DrivetrainState.FWD:
                fl.motorTorque = verticalInput * motorForce;
                fr.motorTorque = verticalInput * motorForce;
                break;
            case DrivetrainState.RWD:
                rl.motorTorque = verticalInput * motorForce;
                rr.motorTorque = verticalInput * motorForce;
                break;
        }   
        currentBrakeForce = isBraking ? brakeForce : 0f;
        Brake();
    }

    private void HandleSteering()
    {
        steerAngle = maxSteerAngle * horizontalInput;
        fl.steerAngle = steerAngle;
        fr.steerAngle = steerAngle;
    }


    private void Brake()
    {
        fl.brakeTorque = currentBrakeForce;
        fr.brakeTorque = currentBrakeForce;
        rl.brakeTorque = currentBrakeForce;
        rr.brakeTorque = currentBrakeForce;
    }

    void FlipCar()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.rotation = Quaternion.Slerp(transform.rotation, defaultRotation, 2f * Time.deltaTime);
    }

    private void OnTriggerStay(Collider collision)
    {
        if(collision.gameObject.tag == "Fence")
        {
            Rigidbody tempRb = collision.gameObject.GetComponent<Rigidbody>();
            tempRb.isKinematic = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Fence")
        {
            Rigidbody tempRb = collision.gameObject.GetComponent<Rigidbody>();
            tempRb.AddExplosionForce(50f * rb.velocity.magnitude, collision.transform.position, 0.3f);
            Destroy(collision.gameObject, 3f);
        }
    }

    #endregion

    #region Wheel Transform Updates

    private void UpdateWheels()
    {
        UpdateWheelVisual(fr, frt);
        UpdateWheelVisual(fl, flt);
        UpdateWheelVisual(rr, rrt);
        UpdateWheelVisual(rl, rlt);
    }

    private void UpdateWheelVisual(WheelCollider col, Transform trans)
    {
        Vector3 pos;
        Quaternion rot;

        col.GetWorldPose(out pos, out rot);
        trans.rotation = rot;
        trans.position = pos;
    }

    #endregion

    #region Update stats to match VehicleStats
    
    void UpdateStats()
    {
        motorForce = vehicleStats.truckSpeed;
        brakeForce = vehicleStats.brakeForce;
        maxSteerAngle = vehicleStats.steerAngle;
        maxSpeed = vehicleStats.maxSpeed;
        drivetrainState = vehicleStats.drivetrainState;

        suspensionSpringRate = vehicleStats.suspensionSpringRate;
        suspensionSpringDamper = vehicleStats.suspensionSpringDamper;
        suspensionTargetPosition = vehicleStats.suspensionTargetPosition;
        
        forwardFrictionAsymSlip = vehicleStats.forwardFrictionAsymSlip;
        forwardFrictionAsymValue = vehicleStats.forwardFrictionAsymValue;
        forwardFrictionExtSlip = vehicleStats.forwardFrictionExtSlip;
        forwardFrictionExtValue = vehicleStats.forwardFrictionExtValue;
        forwardStiffness = vehicleStats.forwardStiffness;

        sidewaysFrictionAsymSlip = vehicleStats.sidewaysFrictionAsymSlip;
        sidewaysFrictionAsymValue = vehicleStats.sidewaysFrictionAsymValue;
        sidewaysFrictionExtSlip = vehicleStats.sidewaysFrictionExtSlip;
        sidewaysFrictionExtValue = vehicleStats.sidewaysFrictionExtValue;
        sidewaysStiffness = vehicleStats.sidewaysStiffness;
    }

    void AssignValuesToObjects()
    {
        suspensionSpring.spring = suspensionSpringRate;
        suspensionSpring.damper = suspensionSpringDamper;
        suspensionSpring.targetPosition = suspensionTargetPosition;

        forwardFrictionCurve.asymptoteSlip = forwardFrictionAsymSlip;
        forwardFrictionCurve.asymptoteValue = forwardFrictionAsymValue;
        forwardFrictionCurve.extremumSlip = forwardFrictionExtSlip;
        forwardFrictionCurve.extremumValue = forwardFrictionExtValue;
        forwardFrictionCurve.stiffness = forwardStiffness;

        sidewaysFrictionCurve.asymptoteSlip = sidewaysFrictionAsymSlip;
        sidewaysFrictionCurve.asymptoteValue = sidewaysFrictionAsymValue;
        sidewaysFrictionCurve.extremumSlip = sidewaysFrictionExtSlip;
        sidewaysFrictionCurve.extremumValue = sidewaysFrictionExtValue;
        sidewaysFrictionCurve.stiffness = sidewaysStiffness;

        for(int i = 0; i < wheelColliders.Length; i++)
        {
            wheelColliders[i].forwardFriction = forwardFrictionCurve;
            wheelColliders[i].sidewaysFriction = sidewaysFrictionCurve;
            wheelColliders[i].suspensionSpring = suspensionSpring;
        }
    }
        

    #endregion
}
