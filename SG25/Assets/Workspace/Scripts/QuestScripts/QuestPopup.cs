using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyGame.QuestSystem;

public class QuestPopup : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject popupPanel;          // 팝업창 전체
    [SerializeField] private Image questIcon;               // 퀘스트 알림 아이콘
    [SerializeField] private Button closeButton;            // 팝업 닫기 버튼
    [SerializeField] private GameObject questPrefab;            // 팝업 닫기 버튼
    [SerializeField] private Transform questContent;            // 팝업 닫기 버튼
    private CenterCameraRaycast playerCtrl;                 // 플레이어 컨트롤러 (커서 제어)
    private bool isPopupVisible = false;                    // 팝업 상태 추적

    private void Awake()
    {
        playerCtrl = FindObjectOfType<CenterCameraRaycast>();

        closeButton.onClick.AddListener(HidePopup);
        // 이벤트 등록
        QuestManager.Instance.OnQuestStarted += UpdateQuestUI;
        QuestManager.Instance.OnQuestCompleted += UpdateQuestUI;

        // 초기 퀘스트 생성 표시
        RefreshQuestList();
    }
    //StartQuest(quest.Id);
    private void Update()
    {
        // Z 키 입력 감지 (토글 기능)
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
    /// ScriptableObject 기반 퀘스트 데이터를 사용해 팝업을 표시합니다.
    /// </summary>
    public void ShowPopup()
    {
        // 팝업창 활성화
        popupPanel.SetActive(true);
        isPopupVisible = true;
        playerCtrl.SetCursorState(isPopupVisible);
        RefreshQuestList();
    }

    public void CreateQuestUI(Quest quest)                             // 개별 퀘스트 UI 생성
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
            buttonText.text = "시작하기";
            startButton.onClick.AddListener(() => QuestManager.Instance.StartQuest(quest.Id));
        }
        if (quest.Status == QuestStatus.InProgress)
        {
            TextMeshProUGUI buttonText = startButton.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = "진행 중";
            startButton.onClick.RemoveListener(() => QuestManager.Instance.StartQuest(quest.Id));
            startButton.image.color = Color.gray;
            //QuestManager.Instance.UpdateQuestProgress(quest.Id);
        }
        if (quest.CheckCompletion())
        {
            TextMeshProUGUI buttonText = startButton.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = "보상 받기";
            startButton.onClick.AddListener(() => QuestManager.Instance.CompleteQuest(quest.Id));
            startButton.image.color = Color.green;
        }
    }

    private void UpdateQuestUI(Quest quest)                             // 퀘스트 상태 변경 시 UI 업데이트
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

        // 기존 UI 제거 (안전하게 null 체크)
        foreach (Transform child in questContent)
        {
            if (child != null)  // 삭제된 객체 방지
            {
                Destroy(child.gameObject);
            }
        }

        // 활성 퀘스트 표시
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