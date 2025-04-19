using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;

public struct NetworkQuestionData : INetworkSerializable
{
    public FixedString512Bytes questionText;
    public FixedString128Bytes category;
    public FixedString128Bytes difficulty;
    public FixedString128Bytes correctAnswer;
    public FixedString128Bytes answer1;
    public FixedString128Bytes answer2;
    public FixedString128Bytes answer3;
    public FixedString128Bytes answer4;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref questionText);
        serializer.SerializeValue(ref category);
        serializer.SerializeValue(ref difficulty);
        serializer.SerializeValue(ref correctAnswer);
        serializer.SerializeValue(ref answer1);
        serializer.SerializeValue(ref answer2);
        serializer.SerializeValue(ref answer3);
        serializer.SerializeValue(ref answer4);
    }

    public string[] GetShuffledAnswers()
    {
        List<string> answers = new();
        if (!string.IsNullOrWhiteSpace(answer1.ToString())) answers.Add(answer1.ToString());
        if (!string.IsNullOrWhiteSpace(answer2.ToString())) answers.Add(answer2.ToString());
        if (!string.IsNullOrWhiteSpace(answer3.ToString())) answers.Add(answer3.ToString());
        if (!string.IsNullOrWhiteSpace(answer4.ToString())) answers.Add(answer4.ToString());
        return answers.OrderBy(_ => UnityEngine.Random.value).ToArray();
    }
}
