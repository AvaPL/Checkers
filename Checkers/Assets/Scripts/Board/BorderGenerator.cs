using UnityEngine;

public class BorderGenerator : MonoBehaviour
{
    public GameObject Border;
    public GameObject Corner;

    private int boardSize;
    private GameObject borderGameObject;
    private Vector3 currentPosition;
    private Quaternion currentRotation;
    private Vector3 currentDirection;

    private void Awake()
    {
        ITilesGenerator tilesGenerator = GetComponent<ITilesGenerator>();
        boardSize = tilesGenerator.BoardSize;
    }

    private void Start()
    {
        CreateBorderGameObject();
        AssignInitialValues();
        CreateBorder();
    }

    private void CreateBorderGameObject()
    {
        borderGameObject = new GameObject("Border");
        borderGameObject.transform.parent = this.gameObject.transform;
        borderGameObject.transform.position = (Vector3.left + Vector3.back);
    }

    private void AssignInitialValues()
    {
        currentPosition = borderGameObject.transform.position;
        currentRotation = borderGameObject.transform.rotation;
        currentDirection = Vector3.forward;
    }

    private void CreateBorder()
    {
        for (var side = 0; side < 4; ++side)
            CreaterBorderLine();
    }

    private void CreaterBorderLine()
    {
        CreateCornerElement();
        for (var i = 0; i < boardSize; ++i)
            CreateBorderElement();
        RotateBy90Degrees();
    }

    private void CreateCornerElement()
    {
        CreateElement(Corner);
    }

    private void CreateElement(GameObject objectToCreate)
    {
        GameObject instantiatedCorner = Instantiate(objectToCreate, currentPosition,
            objectToCreate.transform.rotation * currentRotation, borderGameObject.transform);
        IncrementCurrentPosition();
    }

    private void IncrementCurrentPosition()
    {
        currentPosition += currentDirection;
    }

    private void CreateBorderElement()
    {
        CreateElement(Border);
    }

    private void RotateBy90Degrees()
    {
        Quaternion rotationBy90Degrees = Quaternion.Euler(0, 90, 0);
        currentDirection = rotationBy90Degrees * currentDirection;
        currentRotation *= rotationBy90Degrees;
    }
}