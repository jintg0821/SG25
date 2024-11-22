using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType(typeof(GameManager)) as GameManager;
            }
            return instance;
        }
    }

    [Header("Time")]
    public TextMeshProUGUI timeText; // UI에 현재 게임 시간을 표시하는 텍스트
    public float gameTime = 0.0f; // 게임 세계의 시간을 초로 저장
    public float timeScale = 60.0f; // 현실 시간 1초당 게임 시간 60초 (1분)

    [Header("")]
    public int playerMoney = 10000;

    private UIManager UIManager;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        UIManager = FindObjectOfType<UIManager>();
    }

    void Update()
    {
        // 현실 세계의 경과 시간에 비례하여 게임 시간 증가
        gameTime += Time.deltaTime * timeScale;

        // 게임 시간을 시:분 형식으로 변환
        int hours = (int)(gameTime / 3600); // % 24를 제거하여 24 이상 값도 계산 가능
        int minutes = (int)(gameTime / 60) % 60;

        // 25시가 넘으면 영업 종료
        if (hours >= 25)
        {
            Debug.Log("영업이 종료 되었습니다.");
        }

        // UI 업데이트 (게임 시간 출력)
        timeText.text = string.Format("{0:00}:{1:00}", hours % 24, minutes);
    }


}
