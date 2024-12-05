using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using MyGame.QuestSystem;
using Unity.VisualScripting;
using System;
using System.Reflection;
using static ShopManager;

public class QuestPopup : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject popupPanel;          // 팝업창 전체
    [SerializeField] private Image questIcon;               // 퀘스트 알림 아이콘
    [SerializeField] private TextMeshProUGUI questDescriptionText; // 퀘스트 설명
    [SerializeField] private Button closeButton;            // 팝업 닫기 버튼
    [SerializeField] private GameObject questPrefab;            // 팝업 닫기 버튼
    [SerializeField] private GameObject questContent;            // 팝업 닫기 버튼
    private QuestManager questManager;
    private CenterCameraRaycast playerCtrl;                 // 플레이어 컨트롤러 (커서 제어)
    private bool isPopupVisible = false;                    // 팝업 상태 추적

    private void Awake()
    {
        popupPanel.SetActive(false); // 초기 상태에서 팝업 비활성화
        playerCtrl = FindObjectOfType<CenterCameraRaycast>();
        questManager = GetComponent<QuestManager>();

        // 닫기 버튼 이벤트 등록
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(HidePopup);
        }
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

        GenerateQuestList();
    }

    public void GenerateQuestList()
    {
        foreach (Transform child in questContent.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < questManager.questDataList.Length; i++)
        {
            int index = i;
            GameObject questObj = Instantiate(questPrefab, questContent.transform);

            // Get references to the components
            TextMeshProUGUI questId = questObj.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
            TextMeshProUGUI questTitle = questObj.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
            TextMeshProUGUI questDescription = questObj.transform.GetChild(2).GetComponentInChildren<TextMeshProUGUI>();
            TextMeshProUGUI requiredAmount = questObj.transform.GetChild(3).GetComponentInChildren<TextMeshProUGUI>();
            TextMeshProUGUI rewardAmount = questObj.transform.GetChild(4).GetComponentInChildren<TextMeshProUGUI>();

            if (questObj != null && questManager.questDataList[index] != null)
            {
                questId.text = $"ID : {questManager.questDataList[index].questId}";
                questTitle.text = questManager.questDataList[index].questTitle;
                questDescription.text = questManager.questDataList[index].questDescription; 
                requiredAmount.text = $"남은 개수 : {questManager.questDataList[index].requiredAmount.ToString()}";
                rewardAmount.text =$"보상 : {questManager.questDataList[index].rewardAmount.ToString()}";
                
            }
        }
        
        
    }

    /// <summary>
    /// 팝업을 닫고 상태를 업데이트합니다.
    /// </summary>
    public void HidePopup()
    {
        popupPanel.SetActive(false);
        isPopupVisible = false;
        playerCtrl.SetCursorState(isPopupVisible);
    }
}
