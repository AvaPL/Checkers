using UnityEngine;

public class TileClickDetector : MonoBehaviour
{
    private TileProperties tileProperties;
    private PawnMover pawnMover;

    private void Awake()
    {
        tileProperties = GetComponent<TileProperties>();
    }

    private void Start()
    {
        pawnMover = GetComponentInParent<PawnMover>();
    }

    public void ChildPawnClicked()
    {
        OnMouseDown();
    }

    private void OnMouseDown()
    {
        if (tileProperties.IsOccupied())
            pawnMover.PawnClicked(tileProperties.GetPawn());
        else
            pawnMover.TileClicked(this.gameObject);
    }

    public void ClickTile()
    {
        OnMouseDown();
    }
}