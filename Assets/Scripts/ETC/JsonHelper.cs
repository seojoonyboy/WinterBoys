using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonHelper {
    public static T[] getJsonArray<T>(string json) {
        string newJson = "{ \"array\": " + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.array;
    }

    public static T[] FromJson<T>(string json) {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.array;
    }

    [System.Serializable]
    private class Wrapper<T> {
        public T[] array;
    }
}