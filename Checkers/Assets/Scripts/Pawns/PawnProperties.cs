using System.Collections;
using UnityEngine;

public class PawnProperties : MonoBehaviour, IPawnProperties
{
    public float CrownHeight;
    public float CrownAppearanceSmoothing;
    public float PositionDifferenceTolerance;
    public GameObject Crown;
    public GameObject PromotionParticles;
    public GameObject PawnSelection;

    public PawnColor PawnColor { get; set; }
    public bool IsKing { get; set; }

    private GameObject activePawnSelection;

    public TileIndex GetTileIndex()
    {
        return GetComponentInParent<TileProperties>().GetTileIndex();
    }

    public void PromoteToKing()
    {
        IsKing = true;
        CreatePromotionParticles();
        StartCoroutine(AddCrown());
        PlayPromotionSound();
    }

    private void CreatePromotionParticles()
    {
        GameObject instantiatedParticles = Instantiate(PromotionParticles, transform);
        var particlesDuration = PromotionParticles.GetComponent<ParticleSystem>().main.duration;
        Destroy(instantiatedParticles, particlesDuration * 5);
    }

    private IEnumerator AddCrown()
    {
        var crownTransform = Instantiate(Crown, transform).transform;
        Vector3 targetLocalPosition = crownTransform.localPosition + Vector3.up * CrownHeight;
        while (Vector3.Distance(crownTransform.localPosition, targetLocalPosition) > PositionDifferenceTolerance)
        {
            crownTransform.localPosition = Vector3.Lerp(crownTransform.localPosition, targetLocalPosition,
                CrownAppearanceSmoothing * Time.deltaTime);
            yield return null;
        }
    }

    private void PlayPromotionSound()
    {
        var gameAudio = GameObject.FindGameObjectWithTag("Audio").GetComponent<GameAudio>();
        gameAudio.PlayPromotionSound();
    }

    public void AddPawnSelection()
    {
        if (activePawnSelection != null) return;
        activePawnSelection = Instantiate(PawnSelection, transform);
    }

    public void RemovePawnSelection()
    {
        if (activePawnSelection != null)
            Destroy(activePawnSelection);
    }
}