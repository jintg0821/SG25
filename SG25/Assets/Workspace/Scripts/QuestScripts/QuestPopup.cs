using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class QuestPopup : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject popupPanel;  // 팝업창 전체
    [SerializeField] private Image questIcon;       // 퀘스트 알림 아이콘
    //[SerializeField] private TextMeshProUGUI questTitleText;   // 퀘스트 제목
    [SerializeField] private TextMeshProUGUI questDescriptionText; // 퀘스트 설명
    [SerializeField] private Button closeButton;    // 팝업 닫기 버튼 (추가)
    private CenterCameraRaycast playerCtrl;

    private bool isPopupVisible = false; // 팝업 상태 추적

    private void Awake()
    {
        popupPanel.SetActive(false); // 팝업은 초기 상태에서 비활성화
        playerCtrl = FindObjectOfType<CenterCameraRaycast>();
        // 버튼 클릭 이벤트 등록
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(HidePopup);
        }
    }

    private void Update()
    {
        // Z 키 입력을 감지
        if (Input.GetKeyDown(KeyCode.Z) && !isPopupVisible)
        {
            // Z 키로 팝업 열기
            ShowPopup("New Quest!", "Collect 10 items to complete this quest.");
        }
    }

    public void ShowPopup(string questTitle, string questDescription, Sprite icon = null)
    {
        // 팝업창 활성화
        popupPanel.SetActive(true);
        isPopupVisible = true;
        playerCtrl.SetCursorState(isPopupVisible);

        // 퀘스트 정보 업데이트
        //questTitleText.text = questTitle;
        questDescriptionText.text = questDescription;

        if (icon != null)
        {
            questIcon.sprite = icon;
            questIcon.gameObject.SetActive(true);
        }
        else
        {
            questIcon.gameObject.SetActive(false);
        }
    }

    public void HidePopup()
    {
        // 팝업창 비활성화
        popupPanel.SetActive(false);
        isPopupVisible = false; // 팝업 상태 업데이트
        playerCtrl.SetCursorState(isPopupVisible);
    }
}
