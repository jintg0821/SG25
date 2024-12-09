using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyGame.QuestSystem;

public class QuestPopup : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject popupPanel;          // �˾�â ��ü
    [SerializeField] private Image questIcon;               // ����Ʈ �˸� ������
    [SerializeField] private Button closeButton;            // �˾� �ݱ� ��ư
    [SerializeField] private GameObject questPrefab;            // �˾� �ݱ� ��ư
    [SerializeField] private Transform questContent;            // �˾� �ݱ� ��ư
    private QuestManager questManager;
    private CenterCameraRaycast playerCtrl;                 // �÷��̾� ��Ʈ�ѷ� (Ŀ�� ����)
    private bool isPopupVisible = false;                    // �˾� ���� ����

    private void Awake()
    {
        popupPanel.SetActive(false); // �ʱ� ���¿��� �˾� ��Ȱ��ȭ
        playerCtrl = FindObjectOfType<CenterCameraRaycast>();
        questManager = GetComponent<QuestManager>();

        questManager = QuestManager.Instance;

        closeButton.onClick.AddListener(HidePopup);
        // �̺�Ʈ ���
        questManager.OnQuestStarted += UpdateQuestUI;
        questManager.OnQuestCompleted += UpdateQuestUI;

        // �ʱ� ����Ʈ ���� ǥ��
        RefreshQuestList();
    }

    private void Update()
    {
        // Z Ű �Է� ���� (��� ���)
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (isPopupVisible)
            {
                HidePopup();
            }
            else
            {
                // �׽�Ʈ�� �����ͷ� �˾� ����
                ShowPopup();
            }
        }
    }

    /// <summary>
    /// ScriptableObject ��� ����Ʈ �����͸� ����� �˾��� ǥ���մϴ�.
    /// </summary>
    public void ShowPopup()
    {
        // �˾�â Ȱ��ȭ
        popupPanel.SetActive(true);
        isPopupVisible = true;
        playerCtrl.SetCursorState(isPopupVisible);

        RefreshQuestList();
    }

    private void CreateQuestUI(Quest quest)                             // ���� ����Ʈ UI ����
    {
        GameObject questObj = Instantiate(questPrefab, questContent);

        TextMeshProUGUI idText = questObj.transform.Find("QuestID").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI titleText = questObj.transform.Find("Title").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI descriptionText = questObj.transform.Find("QuestDescription").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI progressText = questObj.transform.Find("CurrentProgress").GetComponent<TextMeshProUGUI>();

        idText.text = quest.Id;
        titleText.text = quest.Title;
        descriptionText.text = quest.Description;
        progressText.text = $"Progress: {quest.GetProgress():P0}";
    }

    private void UpdateQuestUI(Quest quest)                             // ����Ʈ ���� ���� �� UI ������Ʈ
    {
        RefreshQuestList();
    }

    private void RefreshQuestList()                                     // ����Ʈ ��� ���ΰ�ħ
    {
        foreach (Transform child in questContent)                    // ���� UI ����
        {
            Destroy(child.gameObject);
        }

        foreach (var quest in questManager.GetActiveQuest())            // Ȱ�� ����Ʈ ǥ��
        {
            CreateQuestUI(quest);
        }
    }
    public void HidePopup()
    {
        popupPanel.SetActive(false);
        isPopupVisible = false;
        playerCtrl.SetCursorState(isPopupVisible);
    }
}