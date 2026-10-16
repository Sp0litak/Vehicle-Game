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
        if(isWin)
        {
            _resultText.text = _winText;
            _resultText.color = Color.green;
        }
        else
        {
            _resultText.text = _loseText;
            _resultText.color = Color.red;
        }
        _resultText.enabled = true;
    }
}
