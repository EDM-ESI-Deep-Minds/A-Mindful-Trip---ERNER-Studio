using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HeartUIManager : MonoBehaviour
{
    [Header("Heart Sprites")]
    public Sprite fullHeart;
    public Sprite emptyHeart;

    [Header("Heart Animation")]
    public RuntimeAnimatorController heartAnimator;

    public Transform heartContainer;

    private int hearts;
    private int emptyHearts = 0;
    private List<Image> heartImages = new List<Image>(); // Tracks all heart UI images

    public void Awake()
    {
        hearts = GameMode.Instance.GetMaxPlayers() == 2 ? 4 : 3;
        InitializeHearts();
    }

    private void InitializeHearts()
    {
        for (int i = 0; i < hearts; i++)
        {
            GameObject heartGO = new GameObject("Heart" + i);
            heartGO.transform.SetParent(heartContainer, false);

            Image heartImage = heartGO.AddComponent<Image>();
            heartImage.sprite = fullHeart;

            Animator animator = heartGO.AddComponent<Animator>();
            animator.runtimeAnimatorController = heartAnimator;

            heartImages.Add(heartImage);
        }
    }

    public void addHeart()
    {
        if (emptyHearts > 0)
        {
            // Remove the last empty heart
            Transform heartToRemove = heartContainer.GetChild(hearts);
            Destroy(heartToRemove.gameObject);
            heartImages.RemoveAt(hearts); // keep the list accurate
            emptyHearts--;
        }

        hearts++;

        GameObject heartGO = new GameObject("Heart" + heartImages.Count);
        heartGO.transform.SetParent(heartContainer, false);

        Image heartImage = heartGO.AddComponent<Image>();
        heartImage.sprite = fullHeart;

        Animator animator = heartGO.AddComponent<Animator>();
        animator.runtimeAnimatorController = heartAnimator;

        if (heartContainer.childCount > 1)
        {
            // Get the first heart's animation time to sync
            Animator referenceAnimator = heartContainer.GetChild(0).GetComponent<Animator>();
            AnimatorStateInfo stateInfo = referenceAnimator.GetCurrentAnimatorStateInfo(0);
            float normalizedTime = stateInfo.normalizedTime % 1f;

            animator.Play(stateInfo.shortNameHash, 0, normalizedTime);
        }

        heartImages.Add(heartImage);
    }


    public void removeHeart()
    {
        if (hearts <= 0) return;

        hearts--;
        emptyHearts++;

        Transform heart = heartContainer.GetChild(hearts);
        Animator animator = heart.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("PopOut");
        }

        StartCoroutine(SetHeartEmptyAfterAnimation(heart.GetComponent<Image>(), 0.3f));

        // TODO : lossing logic
    }

    private System.Collections.IEnumerator SetHeartEmptyAfterAnimation(Image heartImage, float delay)
    {
        yield return new WaitForSeconds(delay);
        heartImage.sprite = emptyHeart;
    }

    public int getHeart()
    {
        return hearts;
    }
}
