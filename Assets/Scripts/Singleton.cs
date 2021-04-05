using System;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
    
    private static T _instance;
    
    void Awake () {
        SetUpSingleton();
    }
    
    private void SetUpSingleton()
    {
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }
    }

    public static T instance {
        get {
            // Если свойство _instance еще не настроено..
            if (_instance == null)
            {
                // ...попытаться найти объект.
                _instance = FindObjectOfType<T>();
                // Записать сообщение об ошибке в случае неудачи.
                if (_instance == null) {
                    Debug.LogError("Can't find " + typeof(T) + "!");
                }
            }
            // Вернуть экземпляр для использования!
            return _instance;
        }
    }
}