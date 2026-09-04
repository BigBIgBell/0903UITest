using UnityEngine;

/// <summary>比赛结果</summary>
public enum MatchResult
{
    [InspectorName("A队胜")] Win,
    [InspectorName("平")] Draw,
    [InspectorName("B队胜")] Lose
}

/// <summary>本场附加事件</summary>
public enum ExtraEvent
{
    [InspectorName("未发生")] None,
    [InspectorName("落后反超")] Comeback,
    [InspectorName("绝杀")] BuzzerBeater,
    [InspectorName("绝平")] Equalizer
}

/// <summary>
/// 比赛经验配置数据管理（含公式计算）。
/// 强弱分：胜 = ROUND(2 - A/(A+B), 2)；平 = 1；负 = ROUND(2 - B/(A+B), 2)。
/// 本场结果由两队进球自动判断：A多=胜，相等=平，B多=B队胜。
/// </summary>
public class DataManager : MonoBehaviour
{
    /// <summary>全局唯一入口</summary>
    public static DataManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        RefreshUI();
    }

    private void OnValidate()
    {
        RefreshUI();
    }

    /// <summary>更新 UIController经验文本</summary>
    public void RefreshUI()
    {
        if (UIController.Instance == null || !Application.isPlaying)
            return;

        UIController.Instance.SetHeadCoachExp(HeadCoachExperienceFor(CurrentResult).ToString("0.00"));
        UIController.Instance.SetFootballManagerExp(FootballManagerExperienceFor(CurrentResult).ToString("0.00"));
    }

    /// <summary>本场结果：A队进球多 = A队胜，相等 = 平，B队进球多 = B队胜</summary>
    public MatchResult CurrentResult
    {
        get
        {
            if (TeamAScores > TeamBScores) return MatchResult.Win;
            if (TeamAScores == TeamBScores) return MatchResult.Draw;
            return MatchResult.Lose;
        }
    }

    [Header("固定配置")]
    [Tooltip("主教练变量分")]
    public int HeadCoachVariableScore = 15;

    [Tooltip("足球经理变量分")]
    public int FootballManagerVariableScore = 8;

    [Header("结果得分系数")]
    [Tooltip("胜得分系数")]
    public int WinScore = 3;

    [Tooltip("平得分系数")]
    public int DrawScore = 2;

    [Tooltip("负得分系数")]
    public int LoseScore = 1;

    [Header("附加事件")]

    [Tooltip("落后反超/绝杀/绝平；胜局选绝平按未发生处理")]
    public ExtraEvent CurrentEvent = ExtraEvent.None;
    [Header("游戏比赛数值")]
    [Tooltip("A队实力")]
    public int TeamAPower = 2522;

    [Tooltip("B队实力")]
    public int TeamBPower = 2200;

    [Tooltip("A队当前进球")]
    public int TeamAScores = 3;

    [Tooltip("平局进球")]
    public int DrawGoals = 4;

    [Tooltip("B队当前进球")]
    public int TeamBScores = 2;

    // ===================== 强弱分 =====================

    /// <summary>A队胜时的强弱分 = ROUND(2 - A/(A+B), 2)</summary>
    public float WinStrengthScore => Round2(2f - TeamAPower / (float)(TeamAPower + TeamBPower));

    /// <summary>平时的强弱分固定为 1</summary>
    public float DrawStrengthScore => 1f;

    /// <summary>负时的强弱分 = ROUND(2 - B/(A+B), 2)</summary>
    public float LoseStrengthScore => Round2(2f - TeamBPower / (float)(TeamAPower + TeamBPower));

    /// <summary>A、B 队实力差（|A队实力 - B队实力|）</summary>
    public int TeamPowerDifference => Mathf.Abs(TeamAPower - TeamBPower);

    // ===================== 经验计算 =====================

    /// <summary>按当前数据计算主教练经验（不修改界面）</summary>
    public float HeadCoachExperienceFor(MatchResult result)
    {
        float baseScore = HeadCoachVariableScore * StrengthScoreFor(result) + GoalsFor(result);
        int resultScore = ResultScoreFor(result);

        switch (result)
        {
            case MatchResult.Win:
                return baseScore * resultScore + GetEventBonus(result);

            case MatchResult.Draw:
                return baseScore * resultScore + GetEventBonus(result);

            default:
                return baseScore * resultScore;
        }
    }

    /// <summary>按当前数据计算足球经理经验（不修改界面）</summary>
    public float FootballManagerExperienceFor(MatchResult result)
    {
        return (FootballManagerVariableScore * StrengthScoreFor(result) + GoalsFor(result)) * ResultScoreFor(result);
    }

    /// <summary>按指定结果结算，并更新主教练/足球经理经验文本</summary>
    public void ApplyResultToUI(MatchResult result)
    {
        if (UIController.Instance != null)
            UIController.Instance.SetHeadCoachExp(HeadCoachExperienceFor(result).ToString("0.00"));

        if (UIController.Instance != null)
            UIController.Instance.SetFootballManagerExp(FootballManagerExperienceFor(result).ToString("0.00"));
    }

    // ===================== 按比赛结果取值 =====================

    /// <summary>取得指定结果对应的强弱分</summary>
    public float StrengthScoreFor(MatchResult result)
    {
        switch (result)
        {
            case MatchResult.Win: return WinStrengthScore;
            case MatchResult.Draw: return DrawStrengthScore;
            default: return LoseStrengthScore;
        }
    }

    /// <summary>取得指定结果对应的进球数</summary>
    public int GoalsFor(MatchResult result)
    {
        switch (result)
        {
            case MatchResult.Win: return TeamAScores;
            case MatchResult.Draw: return DrawGoals;
            default: return TeamBScores;
        }
    }

    /// <summary>取得指定结果对应的得分（胜得分/平得分/负得分）</summary>
    public int ResultScoreFor(MatchResult result)
    {
        switch (result)
        {
            case MatchResult.Win: return WinScore;
            case MatchResult.Draw: return DrawScore;
            default: return LoseScore;
        }
    }

    // ===================== 内部辅助 =====================

    private const int ComebackBonus = 10;
    private const int BuzzerBeaterBonus = 20;
    private const int EqualizerBonus = 15;

    /// <summary>附加事件按结果生效：胜=落后反超/绝杀，平=绝平；胜局选绝平按未发生处理</summary>
    private int GetEventBonus(MatchResult result)
    {
        if (result == MatchResult.Win)
        {
            if (CurrentEvent == ExtraEvent.Comeback) return ComebackBonus;
            if (CurrentEvent == ExtraEvent.BuzzerBeater) return BuzzerBeaterBonus;
            return 0; // 胜局选了绝平或未发生：不加分
        }

        if (result == MatchResult.Draw && CurrentEvent == ExtraEvent.Equalizer)
            return EqualizerBonus;

        return 0;
    }
    /// <summary>四舍五入保留两位小数</summary>
    private static float Round2(float value)
    {
        return (float)System.Math.Round(value, 2, System.MidpointRounding.AwayFromZero);
    }
}
