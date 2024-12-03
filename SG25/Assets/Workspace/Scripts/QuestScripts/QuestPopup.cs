using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class QuestPopup : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject popupPanel;  // �˾�â ��ü
    [SerializeField] private Image questIcon;       // ����Ʈ �˸� ������
    //[SerializeField] private TextMeshProUGUI questTitleText;   // ����Ʈ ����
    [SerializeField] private TextMeshProUGUI questDescriptionText; // ����Ʈ ����
    [SerializeField] private Button closeButton;    // �˾� �ݱ� ��ư (�߰�)
    private CenterCameraRaycast playerCtrl;

    private bool isPopupVisible = false; // �˾� ���� ����

    private void Awake()
    {
        popupPanel.SetActive(false); // �˾��� �ʱ� ���¿��� ��Ȱ��ȭ
        playerCtrl = FindObjectOfType<CenterCameraRaycast>();
        // ��ư Ŭ�� �̺�Ʈ ���
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(HidePopup);
        }
    }

    private void Update()
    {
        // Z Ű �Է��� ����
        if (Input.GetKeyDown(KeyCode.Z) && !isPopupVisible)
        {
            // Z Ű�� �˾� ����
            ShowPopup("New Quest!", "Collect 10 items to complete this quest.");
        }
    }

    public void ShowPopup(string questTitle, string questDescription, Sprite icon = null)
    {
        // �˾�â Ȱ��ȭ
        popupPanel.SetActive(true);
        isPopupVisible = true;
        playerCtrl.SetCursorState(isPopupVisible);

        // ����Ʈ ���� ������Ʈ
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
        // �˾�â ��Ȱ��ȭ
        popupPanel.SetActive(false);
        isPopupVisible = false; // �˾� ���� ������Ʈ
        playerCtrl.SetCursorState(isPopupVisible);
    }
}
