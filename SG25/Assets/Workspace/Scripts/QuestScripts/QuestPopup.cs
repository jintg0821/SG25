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
    private QuestManager questManager;
    private CenterCameraRaycast playerCtrl;                 // 플레이어 컨트롤러 (커서 제어)
    private bool isPopupVisible = false;                    // 팝업 상태 추적

    private void Awake()
    {
        popupPanel.SetActive(false); // 초기 상태에서 팝업 비활성화
        playerCtrl = FindObjectOfType<CenterCameraRaycast>();
        questManager = GetComponent<QuestManager>();

        questManager = QuestManager.Instance;

        closeButton.onClick.AddListener(HidePopup);
        // 이벤트 등록
        questManager.OnQuestStarted += UpdateQuestUI;
        questManager.OnQuestCompleted += UpdateQuestUI;

        // 초기 퀘스트 생성 표시
        RefreshQuestList();
    }

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
                // 테스트용 데이터로 팝업 열기
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

    private void CreateQuestUI(Quest quest)                             // 개별 퀘스트 UI 생성
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

    private void UpdateQuestUI(Quest quest)                             // 퀘스트 상태 변경 시 UI 업데이트
    {
        RefreshQuestList();
    }

    private void RefreshQuestList()                                     // 퀘스트 목록 새로고침
    {
        foreach (Transform child in questContent)                    // 기존 UI 제거
        {
            Destroy(child.gameObject);
        }

        foreach (var quest in questManager.GetActiveQuest())            // 활성 퀘스트 표시
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