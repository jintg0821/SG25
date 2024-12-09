using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


//UnityMonoBehaviour를 위한 제네릭 싱글톤 구현

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    //이 싱글톤의 유일한 인스턴스
    private static T instance;

    //스레드 안정성을 보장하기 위해 잠금 객체
    private static readonly object _lock = new object();

    //애플리케이션이 종료 중인지 확인하는 플래그
    private static bool isQuitting = false;

    //싱글톤 인스턴스에 대한 공개 접근자
    public static T Instance
    {
        get
        {
            //애플리케이션 종료 중 고스트 객체 생성 방지를 위한 체크
            if (isQuitting)
            {
                Debug.Log($"[싱글톤] '{typeof(T)}' 인스턴스가 애플리케이션 종료 중에 접근 되고 있습니다. 고스트 객체 방지를 위해 null 반환");
                return null;
            }

            //스레드 안정성을 위한 잠금
            lock (_lock)
            {
                //인스턴스가 없으면 찾거나 생성
                if (instance == null)
                {
                    //씬에서 기존 인스턴스 찾기
                    instance = (T)FindObjectOfType(typeof(T));

                    //인스턴스가 없으면 새로 생성
                    if (instance == null)
                    {
                        GameObject singletonObject = new GameObject($"{typeof(T)}(Singleton)");
                        instance = singletonObject.AddComponent<T>();

                        //씬 로드 간 인스턴스 유지
                        DontDestroyOnLoad(singletonObject);

                        Debug.Log($"[싱글톤]{typeof(T)} 의 인스턴스가 DontDestroyOnLoad로 생성되었습니다. ");
                    }
                }
                return instance;
            }
        }
    }

    //Awake 메서드, 싱글톤 초기화
    protected virtual void Awake()
    {
        //인스턴스가 없으면 이것을 인스턴스로 설정
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        //인스턴스가 이미 있고 이것이 아니면 파괴
        else if (instance != null)
        {
            Debug.LogWarning($"[싱글톤] {typeof(T)}의 다른 인스턴스가 이미 존재합니다. 이 중복을 파괴합니다. ");
            Destroy(gameObject);
        }
    }

    //OnApplicationQuit 메서드 종료 플래그를 설정하는데 사용
    protected virtual void OnApplicationQuit()
    {
        isQuitting = true;
    }

    //OnDestroy 메서드 예상치 못한 파괴를 체크 하는 데 사용
    protected virtual void OnDestroy()
    {
        //객체가 파괴되고 있지만 애플리케이션이 종료 중이 아니라면 문제 이기 때문에 로그를 남김
        if (!isQuitting)
        {
            Debug.LogWarning($"[싱글톤] {typeof(T)}의 인스턴스가 애플리케이션 종료가 아닌 시점에서 파괴, 문제가 됨");
        }

        isQuitting = true;
    }

    //다음 프레임에 액션을 실행하는 코루틴
    private System.Collections.IEnumerator ExecuteOnNextFrame(Action action)
    {
        //다음 프레임까지 대기
        yield return null;
        //액션 실행
        action();
    }
}