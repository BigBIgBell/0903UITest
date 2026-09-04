using UnityEditor;
using UnityEngine;

/// <summary>DataManager 的 Inspector：强弱分三种结果常显，经验按自动判断的本场结果显示 A/B 两队</summary>
[CustomEditor(typeof(DataManager))]
public class DataManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DataManager data = (DataManager)target;
        MatchResult result = data.CurrentResult;

        // B 队视角：A队胜时 B 队是负，A队负时 B 队是胜，平时两边都是平
        MatchResult resultB = result;
        if (result == MatchResult.Win) resultB = MatchResult.Lose;
        else if (result == MatchResult.Lose) resultB = MatchResult.Win;

        string resultText = "平";
        if (result == MatchResult.Win)
            resultText = "A队胜";
        else if (result == MatchResult.Lose)
            resultText = "B队胜";

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("本场结果（按进球自动判断）", resultText);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("强弱分（按公式自动计算，不可修改）", EditorStyles.boldLabel);

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.FloatField("A队胜", data.WinStrengthScore);
        EditorGUILayout.FloatField("平", data.DrawStrengthScore);
        EditorGUILayout.FloatField("B队胜", data.LoseStrengthScore);
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("A队经验（自动计算，不可修改）", EditorStyles.boldLabel);

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.FloatField("主教练经验", data.HeadCoachExperienceFor(result));
        EditorGUILayout.FloatField("足球经理经验", data.FootballManagerExperienceFor(result));
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("B队经验（自动计算，不可修改）", EditorStyles.boldLabel);

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.FloatField("主教练经验", data.HeadCoachExperienceFor(resultB));
        EditorGUILayout.FloatField("足球经理经验", data.FootballManagerExperienceFor(resultB));
        EditorGUI.EndDisabledGroup();
    }
}
