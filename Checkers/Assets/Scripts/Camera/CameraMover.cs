using UnityEngine;

public class CameraMover : MonoBehaviour
{
    public float MouseSensitivity;
    public float ScrollSensitivity;
    public float MinViewingAngle;
    public float MaxViewingAngle;
    public float RotationSmoothing;
    public float ScrollSmoothing;
    public float InitialRotation;
    public GameObject Board;

    private int boardSize;
    private float minOffset;
    private float maxOffset;
    private float localOffeset;
    private Vector3 localRotation;

    private void Start()
    {
        var tilesGenerator = Board.GetComponent<ITilesGenerator>();
        boardSize = tilesGenerator.BoardSize;
        SetInitialPosition();
        SetInitialRotation();
    }

    private void SetInitialPosition()
    {
        var initialOffset = boardSize * Mathf.Sqrt(2);
        localOffeset = initialOffset;
        minOffset = initialOffset / 10;
        maxOffset = initialOffset;
        transform.position = initialOffset * Vector3.back;
    }

    private void SetInitialRotation()
    {
        localRotation.y = InitialRotation; //Mouse y axis is x axis in world space.
        var initialRotation = Quaternion.Euler(localRotation.y, 0, 0);
        transform.parent.rotation = initialRotation;
    }

    private void LateUpdate()
    {
        GetMouseInput();
        GetScrollInput();
        ChangePosition();
        ChangeRotation();
    }

    private void GetMouseInput()
    {
        if (!Input.GetMouseButton(1))
            return;
        var xAxis = Input.GetAxis("Mouse X");
        var yAxis = Input.GetAxis("Mouse Y");
        localRotation.x += xAxis * MouseSensitivity;
        localRotation.y -= yAxis * MouseSensitivity;
        localRotation.y = Mathf.Clamp(localRotation.y, MinViewingAngle, MaxViewingAngle);
    }

    private void GetScrollInput()
    {
        var scroll = Input.GetAxis("Mouse ScrollWheel");
        localOffeset -= scroll * ScrollSensitivity;
        localOffeset = Mathf.Clamp(localOffeset, minOffset, maxOffset);
    }

    private void ChangePosition()
    {
        var lerpOffset = Mathf.Lerp(transform.localPosition.z, -localOffeset, ScrollSmoothing * Time.deltaTime);
        transform.localPosition = lerpOffset * Vector3.forward;
    }

    private void ChangeRotation()
    {
        Quaternion targetRotation = Quaternion.Euler(localRotation.y, localRotation.x, 0);
        transform.parent.rotation =
            Quaternion.Lerp(transform.parent.rotation, targetRotation, RotationSmoothing * Time.deltaTime);
    }
}