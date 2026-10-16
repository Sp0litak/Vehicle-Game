using UnityEngine;
using TMPro;

public class GameResultView : MonoBehaviour
{
    [SerializeField] private TMP_Text _resultText;
    [SerializeField] private string _winText;
    [SerializeField] private string _loseText;

    public void Initialize()
    {
        _resultText.enabled = false;
    }

    public void Show(bool isWin)
    {
        _resultText.text = isWin ? _winText : _loseText;
        _resultText.enabled = true;
    }
}
