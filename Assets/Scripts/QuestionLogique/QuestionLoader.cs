using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuestionLoader : MonoBehaviour
{
    private void Start()
    {
        EventTrigger.OnQuestionTile += LoadQuestion;
    }

    void LoadQuestion()
    {
        int category = 9;
        string difficulty = "easy";
        string questionType = "qcm";

        string fileName = $"QuestionBank/category_{category}_{difficulty}_{questionType}";

        TextAsset jsonFile = Resources.Load<TextAsset>(fileName);

        if (jsonFile != null)
        {
            QuestionFile questionFile = JsonUtility.FromJson<QuestionFile>(jsonFile.text);

            if (questionFile != null && questionFile.question.Length > 0)
            {
                Question question = questionFile.question[0];
                Debug.Log("Question: " + question.question);
                Debug.Log("Réponse correcte: " + question.correct_answer);
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
