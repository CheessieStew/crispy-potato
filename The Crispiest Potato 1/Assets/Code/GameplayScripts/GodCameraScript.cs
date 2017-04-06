using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class GodCameraScript : MonoBehaviour {

    public Transform CameraRig;
    public Transform Camera;
    public Vector2 Constraints;
    public Vector2 ConstraintsOrigin;
    public float RotationSensitivity;
    public float ScrollSensitivity;
    public float CameraSpeed;

    float rotationX; //around the rig's right axis
    float rotationY; //around the rig's Y axis;
    Vector3 position;
    Vector3 originPosition;
    Vector3 mouseOrigin;
    float camDistance;

    void OnDrawGizmosSelected()
    {

        Gizmos.color = Color.yellow;
        Matrix4x4 cubeTransform = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.matrix = cubeTransform;
        Gizmos.DrawWireCube(new Vector3(ConstraintsOrigin.x, 1, ConstraintsOrigin.y), new Vector3(Constraints.x, 2, Constraints.y));
    }


    // Use this for initialization
    void Start () {
        originPosition = transform.position +
            new Vector3(ConstraintsOrigin.x, 12, ConstraintsOrigin.y);
        position = CameraRig.position;
        rotationX = CameraRig.localRotation.eulerAngles.x;
	}
	
	// Update is called once per frame
	void Update () {
#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.Escape))
            UnityEditor.EditorApplication.isPaused = true;
#endif
        if (ControlsManager.Instance.FocusNotOnConsole)
        {
            if (Input.GetKey(KeyCode.Mouse1))
            {

                rotationY += Input.GetAxisRaw("Mouse X") * Time.unscaledDeltaTime * RotationSensitivity;
                rotationX -= Input.GetAxisRaw("Mouse Y") * Time.unscaledDeltaTime * RotationSensitivity;

                rotationX = Mathf.Clamp(rotationX, 10, 90);
                CameraRig.localRotation = Quaternion.Euler(rotationX, rotationY, 0);
            }
            if (Input.GetAxisRaw("Mouse ScrollWheel") != 0)
            {
                Camera.localPosition += new Vector3(0, 0, Input.GetAxisRaw("Mouse ScrollWheel") * Time.unscaledDeltaTime * ScrollSensitivity);
                if (Camera.localPosition.z > -0.5f)
                    Camera.localPosition = new Vector3(0, 0, 0.5f);
            }


            position += Quaternion.Euler(0, rotationY, 0) 
                * new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"))
                * Time.unscaledDeltaTime * CameraSpeed;

            position.x = Mathf.Clamp(position.x, 
                originPosition.x - Constraints.x/2,
                originPosition.x + Constraints.x/2);

            position.z = Mathf.Clamp(position.z,
                originPosition.z - Constraints.y/2,
                originPosition.z + Constraints.y/2);

            CameraRig.transform.position = position;
        }
	}
}
