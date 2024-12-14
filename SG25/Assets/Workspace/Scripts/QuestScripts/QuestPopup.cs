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
    private CenterCameraRaycast playerCtrl;                 // �÷��̾� ��Ʈ�ѷ� (Ŀ�� ����)
    private bool isPopupVisible = false;                    // �˾� ���� ����

    private void Awake()
    {
        playerCtrl = FindObjectOfType<CenterCameraRaycast>();

        closeButton.onClick.AddListener(HidePopup);
        // �̺�Ʈ ���
        QuestManager.Instance.OnQuestStarted += UpdateQuestUI;
        QuestManager.Instance.OnQuestCompleted += UpdateQuestUI;

        // �ʱ� ����Ʈ ���� ǥ��
        RefreshQuestList();
    }
    //StartQuest(quest.Id);
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

    public void CreateQuestUI(Quest quest)                             // ���� ����Ʈ UI ����
    {
        GameObject questObj = Instantiate(questPrefab, questContent);

        TextMeshProUGUI idText = questObj.transform.Find("QuestID").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI titleText = questObj.transform.Find("QuestTitle").GetComponent<TextMeshProUGUI>();
        //TextMeshProUGUI descriptionText = questObj.transform.Find("QuestDescription").GetComponent<TextMeshProUGUI>();
        Slider progressSlider = questObj.transform.Find("QuestProgressSlider").GetComponent<Slider>();
        TextMeshProUGUI progressText = questObj.transform.Find("QuestProcess").GetComponent<TextMeshProUGUI>();
        Button startButton = questObj.transform.Find("QuestStartButton").GetComponent<Button>();
        TextMeshProUGUI rewardText = questObj.transform.Find("RewardText").GetComponent<TextMeshProUGUI>();

        idText.text = quest.Id;
        titleText.text = quest.Title;
        //descriptionText.text = quest.Description;
        float progress = quest.GetProgress();
        progressSlider.value = progress;
        progressText.text = $"Progress: {quest.GetProgress():P0}";
        rewardText.text = string.Join("\n", quest.GetRewardDescriptions());

        if (quest.Status == QuestStatus.NotStarted)
        {
            TextMeshProUGUI buttonText = startButton.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = "�����ϱ�";
            startButton.onClick.AddListener(() => QuestManager.Instance.StartQuest(quest.Id));
        }
        if (quest.Status == QuestStatus.InProgress)
        {
            TextMeshProUGUI buttonText = startButton.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = "���� ��";
            startButton.onClick.RemoveListener(() => QuestManager.Instance.StartQuest(quest.Id));
            startButton.image.color = Color.gray;
            //QuestManager.Instance.UpdateQuestProgress(quest.Id);
        }
        if (quest.CheckCompletion())
        {
            TextMeshProUGUI buttonText = startButton.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = "���� �ޱ�";
            startButton.onClick.AddListener(() => QuestManager.Instance.CompleteQuest(quest.Id));
            startButton.image.color = Color.green;
        }
    }

    private void UpdateQuestUI(Quest quest)                             // ����Ʈ ���� ���� �� UI ������Ʈ
    {
        RefreshQuestList();
    }

    private void RefreshQuestList()
    {
        if (questContent == null)
        {
            Debug.LogError("questContent is null or destroyed.");
            return;
        }

        // ���� UI ���� (�����ϰ� null üũ)
        foreach (Transform child in questContent)
        {
            if (child != null)  // ������ ��ü ����
            {
                Destroy(child.gameObject);
            }
        }

        // Ȱ�� ����Ʈ ǥ��
        foreach (var quest in QuestManager.Instance.GetAvailableQuests())
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