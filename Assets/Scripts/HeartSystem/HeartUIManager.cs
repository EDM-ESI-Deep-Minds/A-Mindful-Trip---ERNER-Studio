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

    public GameObject maxheartsMessage;
    public GameObject noNegativeMessage;


    private int hearts;
    private int emptyHearts = 0;
    private List<Image> heartImages = new List<Image>(); // Tracks all heart UI images
    private int maxhearts = 5;

    private bool applyNegativeEffect = true;

    public void Awake()
    {
        RoomUIManager roomUIManager = FindFirstObjectByType<RoomUIManager>();
        int[] nbPlayers = roomUIManager.GetSelectedCharacters();
        foreach(int nb in nbPlayers)
        {
            if (nb != -1)
            {
                hearts++;
            }
        }
        hearts = hearts == 2 ? 4 : 3;

        InitializeHearts();
      //  removeHeart();
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
                int emptyIndex = heartContainer.childCount - emptyHearts;

                Transform heartToRemove = heartContainer.GetChild(emptyIndex);
                Destroy(heartToRemove.gameObject);
                heartImages.RemoveAt(emptyIndex);
                emptyHearts--;
            }

            hearts++;

            GameObject heartGO = new GameObject("Heart" + heartImages.Count);
            heartGO.transform.SetParent(heartContainer, false);

            heartGO.transform.SetSiblingIndex(heartContainer.childCount - emptyHearts - 1);

            Image heartImage = heartGO.AddComponent<Image>();
            heartImage.sprite = fullHeart;

            Animator animator = heartGO.AddComponent<Animator>();
            animator.runtimeAnimatorController = heartAnimator;

            if (heartContainer.childCount > 1)
            {
                Animator referenceAnimator = heartContainer.GetChild(0).GetComponent<Animator>();
                AnimatorStateInfo stateInfo = referenceAnimator.GetCurrentAnimatorStateInfo(0);
                float normalizedTime = stateInfo.normalizedTime % 1f;

                animator.Play(stateInfo.shortNameHash, 0, normalizedTime);
            }

            int insertIndex = heartGO.transform.GetSiblingIndex();
            if (insertIndex >= heartImages.Count)
            {
                heartImages.Add(heartImage);
            }
            else
            {
                heartImages.Insert(insertIndex, heartImage);
            }
        }
        else
        {
            maxheartsMessage.SetActive(true);
            StartCoroutine(HideMessageAfterDelay());
            Debug.Log("Max hearts reached");
        }
    }

    private System.Collections.IEnumerator HideMessageAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        maxheartsMessage.SetActive(false);
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
            AudioManager.instance?.PlaySFX(AudioManager.instance.soulShatterSFX);
           
            Debug.Log("No more hearts to remove");
            if (GameOverManager.Instance != null)
            { 
                Debug.Log("GAMEOVER");
                //NetworkManager.Singleton.SceneManager.LoadScene("GameOver", LoadSceneMode.Single);
                GameOverManager.Instance.TriggerGameOver(); //  delay

            }
        }else
        {
            AudioManager.instance?.PlaySFX(AudioManager.instance.damageTakenSFX);
            if (hearts == 1)
            {
                AskForHelp askForHelp = FindFirstObjectByType<AskForHelp>();
                askForHelp.HelpHimB.gameObject.SetActive(false);
            }
        }

    }

    public void removeAllHearts()
    {
        while (hearts>0)
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

        AudioManager.instance?.PlaySFX(AudioManager.instance.soulShatterSFX);

        Debug.Log("No more hearts to remove");
        if (GameOverManager.Instance != null)
        {
            Debug.Log("GAMEOVER");
            //NetworkManager.Singleton.SceneManager.LoadScene("GameOver", LoadSceneMode.Single);
            GameOverManager.Instance.TriggerGameOver(); //  delay

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

    public int getMaxHearts()
    {
        return maxhearts;
    }

    public void showNoNegative()
    {
        noNegativeMessage.SetActive(true);
        applyNegativeEffect = false;
    }

    public void hideNoNegative()
    {
        noNegativeMessage.SetActive(false);
        applyNegativeEffect = true;
    }

    public bool getApplyNegativeEffect()
    {
        return applyNegativeEffect;
    }
}


