using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
            int randomQuestion = Random.Range(0, questionFile.question.Length);

            Question question = questionFile.question[randomQuestion];
            Debug.Log("Question: " + question.question);
            Debug.Log("Réponses possibles : " + string.Join(", ", GetAnswers(question)));
            Debug.Log("Réponse correcte: " + question.correct_answer);
        }
        else
        {
            questionType = "qcm";
        }
    }

    public string[] GetAnswers(Question question)
    {
        List<string> allAnswers = new List<string>();
        allAnswers.Add(question.correct_answer);
        allAnswers.AddRange(question.incorrect_answers);
        allAnswers = allAnswers.OrderBy(x => Random.value).ToList();
        return allAnswers.ToArray();
    }
}
