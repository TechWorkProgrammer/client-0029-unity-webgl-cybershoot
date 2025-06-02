using System;
using UnityEngine;

[Serializable]
public class  PlayerData {
    public long id;
    public string username;
    public int total_score;
}

[Serializable]
public class Token {
    public string accessToken = "";
    public string refreshToken = "";
}

[System.Serializable]
public class UserDataResponse {
    public string message;
    public Payload payload;
}

[System.Serializable]
public class Payload {
    public PlayerData user;
}