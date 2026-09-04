using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ExperienceTestUI : MonoBehaviour
{
    [Header("测试控件引用")]
    [Tooltip("0=主教练经验，1=足球经理经验")]
    [SerializeField]
    private Dropdown expDropdown;

    [Tooltip("输入新的经验数值")]
    [SerializeField]
    private TMP_InputField valueInput;

    [Tooltip("点击后生效的“更改”按钮")]
    [SerializeField]
    private Button changeButton;

    [Header("快捷键")]
    [Tooltip("按 ~ 时显示/隐藏的面板")]
    [SerializeField]
    private GameObject panelRoot;

    private void Awake()
    {
        if (panelRoot == null)
            panelRoot = gameObject;
    }

    private void OnEnable()
    {
        if (changeButton != null)
            changeButton.onClick.AddListener(ApplyChange);
    }

    private void OnDisable()
    {
        if (changeButton != null)
            changeButton.onClick.RemoveListener(ApplyChange);
    }

    private void Start()
    {

    }

    private void Update()
    {
        // 按 ~ 键显示/隐藏测试面板
        if (Input.GetKeyDown(KeyCode.BackQuote))
            TogglePanel();
    }

    /// <summary>显示/隐藏测试面板</summary>
    public void TogglePanel()
    {
        if (panelRoot == null)
            return;

        if (panelRoot.activeSelf)
        {
            // 隐藏面板
            panelRoot.SetActive(false);
           
        }
        else
        {
            panelRoot.SetActive(true);
        }
    }

    /// <summary>更新经验文本</summary>
    public void ApplyChange()
    {
        if (UIController.Instance == null)
        {
            Debug.LogWarning("ExperienceTestUI：找不到 UIController，请确认场景中存在挂有 UIController 的物体", this);
            return;
        }

        if (expDropdown == null || valueInput == null)
            return;

        string raw = valueInput.text.Trim();
        if (!int.TryParse(raw, out int value))
        {
            Debug.LogWarning($"ExperienceTestUI：输入的不是有效数字：\"{raw}\"", this);
            return;
        }

        if (expDropdown.value == 0)
        {
            UIController.Instance.SetHeadCoachExp(value);
            Debug.Log($"[经验测试] 主教练经验已设为 {value}");
        }
        else
        {
            UIController.Instance.SetFootballManagerExp(value);
            Debug.Log($"[经验测试] 足球经理经验已设为 {value}");
        }
    }
}
