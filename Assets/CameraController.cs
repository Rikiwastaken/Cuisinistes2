using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private float rotationX = 0f;
    private float rotationY = 0f;


    public float sensitivityX = 15f;
    public float sensitivityY = 15f;

    InputAction LookAction;

    public Transform CameraTransform;

    public float minVerticalAngle = -80f;
    public float maxVerticalAngle = 80f;


    void Start()
    {
        LookAction = InputSystem.actions.FindAction("Look");
        transform.rotation = Quaternion.identity;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mouseDelta = LookAction.ReadValue<Vector2>();

        float mouseX = mouseDelta.x * sensitivityX * Time.deltaTime * DataScript.instance.Options.sensibility;
        float mouseY = mouseDelta.y * sensitivityY * Time.deltaTime * DataScript.instance.Options.sensibility;


        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);

        CameraTransform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);

        rotationY += mouseX;
        transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
    }
}
