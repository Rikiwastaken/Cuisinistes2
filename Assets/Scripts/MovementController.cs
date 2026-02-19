using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour
{

    public static MovementController instance;

    [Header("UI")]
    public TextMeshProUGUI HPTMP;
    public TextMeshProUGUI WaveTMP;

    [Header("Movement")]
    public Transform CameraTransform;
    private Rigidbody rb;

    private InputAction MoveAction;

    public float speed;

    public Vector3 StartPos;
    private int delaybetweenfootstepscnt;
    public float delaybetweenfootsteps;
    public Vector3 basepos;

    [Header("Jump")]
    private InputAction JumpAction;
    public float JumpVerticalSpeed;
    public float DoubleJumpSpeedRatio;
    public bool touchingground;

    public bool jumpavailable;
    public bool doublejumpavailable;
    public int justjumpedcounter;
    public float jumpduration;
    public float downacceleration;

    public bool pressedjump;

    [Header("Sounds")]
    public List<AudioClip> JumpSFX;
    public List<AudioClip> AirStepSFX;
    public List<AudioClip> TouchGroundSFX;
    public List<AudioClip> FootStep;
    public List<AudioClip> playerDamageSounds;

    private bool previoustouchingground;

    private bool textshown;

    private UpgradeScript UpgradeScript;

    [Header("MapSelection")]

    public List<GameObject> Maps;

    private HealthScript healthScript;

    private void OnTriggerStay(Collider other)
    {
        if (LayerMask.NameToLayer("Ground") == other.gameObject.layer && justjumpedcounter <= 0)
        {
            touchingground = true;

        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (LayerMask.NameToLayer("Ground") == other.gameObject.layer)
        {
            touchingground = false;

        }

    }


    private void Awake()
    {
        instance = this;
        Cursor.lockState = CursorLockMode.Locked;

        Maps[UnityEngine.Random.Range(0, Maps.Count)].SetActive(true);

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        MoveAction = InputSystem.actions.FindAction("Move");
        JumpAction = InputSystem.actions.FindAction("Jump");
        UpgradeScript = GetComponent<UpgradeScript>();
        StartPos = transform.position;
        basepos = transform.position;
        healthScript = GetComponent<HealthScript>();
    }


    void Update()
    {
        //bonus
        if (UpgradeScript.gettingbonus || healthScript.HP <= 0)
        {
            return;
        }

        if (!textshown)
        {
            textshown = true;
            GetComponent<TitleText>().StartTitleText();
        }

        if (Mathf.Abs(transform.position.y - basepos.y) >= 30)
        {
            transform.position = basepos;
        }

        // movement;

        Vector2 MoveValue = MoveAction.ReadValue<Vector2>();

        if (MoveValue.magnitude != 0)
        {
            Vector3 movement = Vector3.zero;
            movement = new Vector3(MoveValue.x * speed, 0.0f, MoveValue.y * speed);

            movement = Quaternion.Euler(0, CameraTransform.eulerAngles.y, 0) * movement;



            movement.y = rb.linearVelocity.y;


            rb.linearVelocity = movement;

            if (delaybetweenfootstepscnt <= 0)
            {
                delaybetweenfootstepscnt = 15;
                if (touchingground)
                {
                    SoundManager.instance.PlaySFXFromList(FootStep, 0.2f, transform);
                }


            }
            else
            {
                delaybetweenfootstepscnt--;
            }

        }
        else
        {
            Vector3 targetspeed = new Vector3(0f, rb.linearVelocity.y, 0f);
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, targetspeed, 0.5f);
        }


        // jump
        if (touchingground)
        {
            jumpavailable = true;
            doublejumpavailable = true;
        }

        if (justjumpedcounter > 0)
        {
            justjumpedcounter--;
        }
        //else if (!touchingground)
        //{
        //    rb.AddForce(new Vector3(0, -downacceleration, 0));
        //}

        float jumpValue = JumpAction.ReadValue<float>();
        if (jumpValue != 0)
        {
            if (jumpavailable)
            {

                jumpavailable = false;
                pressedjump = true;
                justjumpedcounter = (int)(jumpduration / Time.deltaTime);
                SoundManager.instance.PlaySFXFromList(JumpSFX, 0.05f, transform);
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, JumpVerticalSpeed, rb.linearVelocity.z);

            }

            else if (doublejumpavailable && !pressedjump)
            {
                doublejumpavailable = false;
                pressedjump = true;
                justjumpedcounter = (int)(jumpduration / Time.deltaTime);
                SoundManager.instance.PlaySFXFromList(AirStepSFX, 0.05f, transform);
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, JumpVerticalSpeed * DoubleJumpSpeedRatio, rb.linearVelocity.z);
            }
        }
        else
        {
            if (justjumpedcounter > 0)
            {
                if (doublejumpavailable)
                {
                    rb.linearVelocity = new Vector3(rb.linearVelocity.x, JumpVerticalSpeed, rb.linearVelocity.z);
                }
                else
                {
                    rb.linearVelocity = new Vector3(rb.linearVelocity.x, JumpVerticalSpeed * DoubleJumpSpeedRatio, rb.linearVelocity.z);
                }

            }

            if (pressedjump)
            {
                pressedjump = false;
            }

        }



        if (previoustouchingground != touchingground)
        {
            if (touchingground)
            {
                SoundManager.instance.PlaySFXFromList(TouchGroundSFX, 0.05f, transform);
            }
            previoustouchingground = touchingground;
        }
    }

    private void LateUpdate()
    {
        if (justjumpedcounter > 0)
        {
            touchingground = false;
        }
        if (touchingground)
        {
            rb.AddForce(new Vector3(0, -2f, 0), ForceMode.VelocityChange);
        }
    }
}
