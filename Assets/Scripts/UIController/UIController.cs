using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }

    [Header("经验文本引用")]
    [Tooltip("足球经理经验的数值显示文本")]
    [SerializeField]
    private Text footballManagerExpText;

    [Tooltip("主教练经验的数值显示文本")]
    [SerializeField]
    private Text headCoachExpText;

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
        if (footballManagerExpText == null)
            Debug.LogWarning("UIController：足球经理经验文本未赋值", this);

        if (headCoachExpText == null)
            Debug.LogWarning("UIController：主教练经验文本未赋值", this);
    }

    // ======================= 足球经理经验 =======================

    /// <summary>设置足球经理经验文本</summary>
    public void SetFootballManagerExp(int value)
    {
        SetText(footballManagerExpText, value.ToString());
    }

    /// <summary>设置足球经理经验文本</summary>
    public void SetFootballManagerExp(string value)
    {
        SetText(footballManagerExpText, value);
    }

    // ======================= 主教练经验 =======================

    /// <summary>设置主教练经验文本</summary>
    public void SetHeadCoachExp(int value)
    {
        SetText(headCoachExpText, value.ToString());
    }

    /// <summary>设置主教练经验文本</summary>
    public void SetHeadCoachExp(string value)
    {
        SetText(headCoachExpText, value);
    }

    private void SetText(Text target, string value)
    {
        if (target != null)
            target.text = value;
    }
}
