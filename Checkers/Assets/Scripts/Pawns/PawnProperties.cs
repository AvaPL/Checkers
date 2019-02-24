using System.Collections;
using UnityEngine;

public class PawnProperties : MonoBehaviour
{
    public PawnColor PawnColor;
    public bool IsKing;
    public float CrownHeight;
    public float CrownAppearanceSmoothing;
    public float PositionDifferenceTolerance;
    public GameObject Crown;
    public GameObject PromotionParticles;

    public TileIndex GetTileIndex()
    {
        return GetComponentInParent<TileProperties>().GetTileIndex();
    }

    public void PromoteToKing()
    {
        IsKing = true;
        CreatePromotionParticles();
        StartCoroutine(AddCrown());
        Debug.Log("Pawn promoted to king.");
    }

    private void CreatePromotionParticles()
    {
        GameObject instantiatedParticles = Instantiate(PromotionParticles, transform);
        var particlesDuration = PromotionParticles.GetComponent<ParticleSystem>().main.duration;
        Destroy(instantiatedParticles, particlesDuration * 5);
    }

    IEnumerator AddCrown()
    {
        var crownTransform = Instantiate(Crown, transform).transform;
        Vector3 targetPosition = crownTransform.position + Vector3.up * CrownHeight;
        while (Vector3.Distance(crownTransform.position, targetPosition) > PositionDifferenceTolerance)
        {
            crownTransform.position = Vector3.Lerp(crownTransform.position, targetPosition,
                CrownAppearanceSmoothing * Time.deltaTime);
            yield return null;
        }
    }
}