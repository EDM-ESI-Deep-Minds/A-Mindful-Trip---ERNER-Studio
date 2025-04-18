using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using Unity.Netcode;

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
    private int maxhearts = GameMode.Instance.GetMaxPlayers() == 2 ? 1 : 6;

    public void Awake()
    {
        hearts = maxhearts;
        InitializeHearts();
        removeHeart();
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
        if (hearts < maxhearts)
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
        else
        {
            Debug.Log("Max hearts reached");
            // ui afficher un message au player "max heart reached"
        }
    }

    public void removeHeart()
    {
        if (hearts > 0)
        {

            hearts--;
           
            emptyHearts++;

            Transform heart = heartContainer.GetChild(hearts);
            Animator animator = heart.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("PopOut");
            }
          
            StartCoroutine(SetHeartEmptyAfterAnimation(heart.GetComponent<Image>(), 0.3f));
        }
        if (hearts == 0)
        {
           
           
            Debug.Log("No more hearts to removeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee");
             if (GameOverManager.Instance != null) { 
            Debug.Log("GAMEOVERrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrr");
                //NetworkManager.Singleton.SceneManager.LoadScene("GameOver", LoadSceneMode.Single);
                GameOverManager.Instance.TriggerGameOverScene(); //  delay

            }
        }

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


