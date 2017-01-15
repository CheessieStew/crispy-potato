using UnityEngine;
using System.Collections;

public class GodCameraScript : MonoBehaviour {

    public Transform CameraRig;
    public Transform Camera;
    public Vector2 Constraints;
    public Vector2 ConstraintsOrigin;
    public float RotationSensitivity;
    public float ScrollSensitivity;

    float rotationX; //around the rig's right axis
    float rotationY; //around the rig's Y axis;
    Vector3 position;
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
        position = new Vector3(ConstraintsOrigin.x, 2, ConstraintsOrigin.y);
        rotationX = CameraRig.localRotation.eulerAngles.x;
	}
	
	// Update is called once per frame
	void Update () {
#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.Escape))
            UnityEditor.EditorApplication.isPaused = true;
#endif
        if (Input.GetKey(KeyCode.Mouse1))
        {
            
            rotationY -= Input.GetAxis("Mouse X") * Time.deltaTime * RotationSensitivity;
            rotationX -= Input.GetAxis("Mouse Y") * Time.deltaTime * RotationSensitivity;

            rotationX = Mathf.Clamp(rotationX, 10, 90);
            CameraRig.localRotation = Quaternion.Euler(rotationX, rotationY, 0);
        }
        if (Input.GetAxisRaw("Mouse ScrollWheel") != 0)
            Camera.localPosition += new Vector3(0, 0, Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * ScrollSensitivity);

        position += Quaternion.Euler(0, rotationY, 0) * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));


        CameraRig.transform.position = position;
	}
}
