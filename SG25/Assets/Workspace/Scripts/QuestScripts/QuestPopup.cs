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
    [SerializeField] private GameObject popupPanel;          // �˾�â ��ü
    [SerializeField] private Image questIcon;               // ����Ʈ �˸� ������
    [SerializeField] private TextMeshProUGUI questDescriptionText; // ����Ʈ ����
    [SerializeField] private Button closeButton;            // �˾� �ݱ� ��ư
    [SerializeField] private GameObject questPrefab;            // �˾� �ݱ� ��ư
    [SerializeField] private GameObject questContent;            // �˾� �ݱ� ��ư
    private QuestManager questManager;
    private CenterCameraRaycast playerCtrl;                 // �÷��̾� ��Ʈ�ѷ� (Ŀ�� ����)
    private bool isPopupVisible = false;                    // �˾� ���� ����

    private void Awake()
    {
        popupPanel.SetActive(false); // �ʱ� ���¿��� �˾� ��Ȱ��ȭ
        playerCtrl = FindObjectOfType<CenterCameraRaycast>();
        questManager = GetComponent<QuestManager>();

        // �ݱ� ��ư �̺�Ʈ ���
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(HidePopup);
        }
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
                requiredAmount.text = $"���� ���� : {questManager.questDataList[index].requiredAmount.ToString()}";
                rewardAmount.text =$"���� : {questManager.questDataList[index].rewardAmount.ToString()}";
                
            }
        }
        
        
    }

    /// <summary>
    /// �˾��� �ݰ� ���¸� ������Ʈ�մϴ�.
    /// </summary>
    public void HidePopup()
    {
        popupPanel.SetActive(false);
        isPopupVisible = false;
        playerCtrl.SetCursorState(isPopupVisible);
    }
}
