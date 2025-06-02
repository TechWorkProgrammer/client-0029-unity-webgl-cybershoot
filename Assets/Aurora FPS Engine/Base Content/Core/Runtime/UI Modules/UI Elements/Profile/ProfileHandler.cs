using UnityEngine;
using TMPro;

public class ProfileHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI usernameText;
    [SerializeField] private TextMeshProUGUI totalScoreText;

    // Update is called once per frame
    public void DisplayProfile()
    {
        usernameText.text = "Username: " + ScoreManager.Instance.playerData.username;
        totalScoreText.text = "$cybers: " + ScoreManager.Instance.playerData.total_score.ToString();
    }
    // 01101101 01100011 01101100 01100001 01110010 01100101 01101110 00100000 01101100 01101111 00100000 01110111 01100001 01110010 01101110 01100001 00100000 01100001 01110000 01100001
}
