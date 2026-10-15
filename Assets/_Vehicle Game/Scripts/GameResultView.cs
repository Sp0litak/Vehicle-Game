using UnityEngine;
using UnityEngine.UI;

public class GameResultView : MonoBehaviour
{
    [SerializeField] private Text _resultText;

    public void Initialize()
    {
        _resultText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        _resultText.text = string.Empty;
    }

    public void Show(bool isWin)
    {
        _resultText.text = isWin ? "You win" : "You lose";
    }
}
