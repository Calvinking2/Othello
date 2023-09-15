using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms.Impl;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI toptext;
    [SerializeField] private TextMeshProUGUI blackScoreText;
    [SerializeField] private TextMeshProUGUI whiteScoreText;
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private Image blackOverlay;
    [SerializeField] private RectTransform playAgainButton;

    public void setPlayerText(Player currentPlayer)
    {
        if(currentPlayer == Player.Black)
        {
            toptext.text = "Black's Turn <sprite name=DiscBlackUp>";
        }else if(currentPlayer == Player.White)
        {
            toptext.text = "White's Turn <sprite name=DiscWhiteUp>";
        }
    }

    public void SetSkippedText(Player skippedPlayer)
    {
        if(skippedPlayer == Player.Black)
        {
            toptext.text = "Black Cannot Move! <sprite name=DiscBlackUp>";
        }else if(skippedPlayer == Player.White)
        {
            toptext.text = "White Cannot Move! <sprite name=DiscWhiteUp>";
        }
    }

    public void SetTopText(string massage)
    {
        toptext.text = massage;
    }

    public IEnumerator AnimateTopText()
    {
        toptext.transform.LeanScale(Vector3.one*1.2f, 0.25f).setLoopPingPong(4);
        yield return new WaitForSeconds(2);
    }
    private IEnumerator ScaleDown(RectTransform rect)
    {
        rect.LeanScale(Vector3.zero, 0.2f);
        yield return new WaitForSeconds(0.2f);
        rect.gameObject.SetActive(false);   
    }

    private IEnumerator ScaleUp(RectTransform rect)
    {
        rect.gameObject.SetActive(true);
        rect.localScale = Vector3.zero;
        rect.LeanScale(Vector3.one,0.2f);
        yield return new WaitForSeconds(0.2f);
    }

    public IEnumerator ShowScoreText()
    {
        yield return ScaleDown(toptext.rectTransform);
        yield return ScaleUp(blackScoreText.rectTransform);
        yield return ScaleUp(whiteScoreText.rectTransform);
    }
    public void SetBlackScoreText(int score)
    {
        blackScoreText.text = $"<sprite name=DiscBlackUp>{score}";
    }
    public void SetWhiteScoreText(int score)
    {
        whiteScoreText.text = $"<sprite name=DiscWhiteUp>{score}";
    }
    private IEnumerator ShowOverlay()
    {
        blackOverlay.gameObject.SetActive(true);
        blackOverlay.color = Color.clear;
        blackOverlay.rectTransform.LeanAlpha(0.8f, 1);
        yield return new WaitForSeconds(1);
    }
    private IEnumerator HideOverlay()
    {
        blackOverlay.rectTransform.LeanAlpha(0, 1);
        yield return new WaitForSeconds(1);
        blackOverlay.gameObject.SetActive(false);
    }
    private IEnumerator MoveScoresDown()
    {
        blackScoreText.rectTransform.LeanMoveY(0, 0.5f);
        whiteScoreText.rectTransform.LeanMoveY(0, 0.5f);
        yield return new WaitForSeconds(0.5f);
    }
    public void setWinnerText(Player winner)
    {
        switch(winner)
        {
            case Player.Black:
                winnerText.text = "Black Won!";
                break;
            case Player.White: 
                winnerText.text = "White Won!";
                break;
            case Player.None:
                winnerText.text = "It's a Tie";
                break;
        }
    }
    public IEnumerator ShowEndScreen()
    {
        yield return ShowOverlay();
        yield return MoveScoresDown();
        yield return ScaleUp(winnerText.rectTransform);
        yield return ScaleUp(playAgainButton);
    }

    public IEnumerator HideEndScreen()
    {
        StartCoroutine(ScaleDown(winnerText.rectTransform));
        StartCoroutine(ScaleDown(blackScoreText.rectTransform));
        StartCoroutine(ScaleDown(whiteScoreText.rectTransform));
        StartCoroutine(ScaleDown(playAgainButton));

        yield return new WaitForSeconds(0.5f);
        yield return HideOverlay();
    }
}
