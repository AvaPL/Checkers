using UnityEngine;

public class PawnClickDetector : MonoBehaviour
{
    private void OnMouseDown()
    {
        GetComponentInParent<TileClickDetector>().ChildPawnClicked();
    }
}
