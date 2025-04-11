using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuestionLoader : MonoBehaviour
{
    void Start()
    {
        LoadQuestion();
    }

    void LoadQuestion()
    {
        // Charge le fichier JSON depuis Resources
        TextAsset jsonFile = Resources.Load<TextAsset>("QuestionBank/category_9_easy_qcm");

        if (jsonFile != null)
        {
            // Désérialisation du JSON en QuestionFile
            QuestionFile questionFile = JsonUtility.FromJson<QuestionFile>(jsonFile.text);

            if (questionFile != null && questionFile.question.Length > 0)
            {
                // Exemple : afficher la première question
                Question q = questionFile.question[0];
                Debug.Log("Question: " + q.question);
                Debug.Log("Réponse correcte: " + q.correct_answer);
            }
            else
            {
                Debug.LogWarning("Aucune question trouvée dans le fichier.");
            }
        }
        else
        {
            Debug.LogError("Fichier JSON non trouvé !");
        }
    }
    
}
