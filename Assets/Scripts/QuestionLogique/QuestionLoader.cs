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
            // D�s�rialisation du JSON en QuestionFile
            QuestionFile questionFile = JsonUtility.FromJson<QuestionFile>(jsonFile.text);

            if (questionFile != null && questionFile.question.Length > 0)
            {
                // Exemple : afficher la premi�re question
                Question q = questionFile.question[0];
                Debug.Log("Question: " + q.question);
                Debug.Log("R�ponse correcte: " + q.correct_answer);
            }
            else
            {
                Debug.LogWarning("Aucune question trouv�e dans le fichier.");
            }
        }
        else
        {
            Debug.LogError("Fichier JSON non trouv� !");
        }
    }
    
}
