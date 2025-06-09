using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderEntry : MonoBehaviour
{
    public TextMeshProUGUI nm, rank, score;
    public Image img;
    public Color myColor, oohColor;
    public void SetEntry(string _nm, int _rank, int _score, bool isMine)
    {
        img.color = oohColor;
        nm.text = _nm;
        if (_rank == 1)
        {
            rank.text = $"{_rank}st";
        }
        else if (_rank == 2)
        {
            rank.text = $"{_rank}nd";
        }
        else if (_rank == 3)
        {
            rank.text = $"{_rank}rd";
        }
        else
        {
            rank.text = $"{_rank}th";
        }
        score.text = _score.ToString();
        if (isMine) {
            nm.fontStyle = FontStyles.Italic | FontStyles.Underline;
        }
    }

}
