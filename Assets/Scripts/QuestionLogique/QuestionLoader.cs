using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class QuestionLoader : MonoBehaviour
{
    public static QuestionLoader Instance { get; private set; }

    public static List<string> askedQuestions = new List<string>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // private void Start()
    // {
    //     EventTrigger.OnQuestionTile += LoadQuestion;
    // }

    public Question LoadQuestion(int category, string difficulty, string questionType)
    {
        string fileName = $"QuestionBank/category_{category}_{difficulty}_{questionType}";
        TextAsset jsonFile = Resources.Load<TextAsset>(fileName);

        if (jsonFile == null && questionType != "qcm")
        {
            questionType = "qcm";
            fileName = $"QuestionBank/category_{category}_{difficulty}_{questionType}";
            jsonFile = Resources.Load<TextAsset>(fileName);
        }

        if (jsonFile != null)
        {
            QuestionFile questionFile = JsonUtility.FromJson<QuestionFile>(jsonFile.text);
            Question question;
            do
            {
               question = questionFile.question[Random.Range(0, questionFile.question.Length)];
            } while (askedQuestions.Contains(question.question));

            return  question;
        }

        Debug.LogError("Failed to load question file.");
        return null;
    }

    public string[] GetAnswers(Question question)
    {
        List<string> allAnswers = new List<string>();
        allAnswers.Add(question.correct_answer);
        allAnswers.AddRange(question.incorrect_answers);
        allAnswers = allAnswers.OrderBy(x => Random.value).ToList();
        return allAnswers.ToArray();
    }
    public string GetRandomQuestionType()
    {
        string[] types = { "qcm", "tf" };
        int index = Random.Range(0, types.Length);
        return types[index];
    }
    //public int GetRandomCategory()
    //{
    //    return UnityEngine.Random.Range(9, 33); //TODO make it personilized with the player profile
    //}
   
}
