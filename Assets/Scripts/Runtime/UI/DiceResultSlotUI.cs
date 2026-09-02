using UnityEngine;
using UnityEngine.UI;

public class DiceResultSlotUI : MonoBehaviour
{
    [SerializeField] private Image _diceImage;

    public void Show(Dice dice, Sprite[] d4Sprites, Sprite[] d6Sprites)
    {
        if (dice == null)
        {
            return;
        }

        gameObject.SetActive(true);

        Sprite[] sprites;

        if (dice.DiceSides == 4)
        {
            sprites = d4Sprites;
        }
        else
        {
            sprites = d6Sprites;
        }

        if (sprites == null || sprites.Length == 0)
        {
            return;
        }

        int result = dice.CurrentResult;

        if (result <= 0)
        {
            result = dice.MinValue;
        }

        if (result > sprites.Length)
        {
            return;
        }

        _diceImage.sprite = sprites[result - 1];
    }

    public void ShowResult(int result, Dice dice, Sprite[] d4Sprites, Sprite[] d6Sprites)
    {
        if (dice == null)
        {
            return;
        }

        gameObject.SetActive(true);

        Sprite[] sprites;

        if (dice.DiceSides == 4)
        {
            sprites = d4Sprites;
        }
        else
        {
            sprites = d6Sprites;
        }

        if (sprites == null || sprites.Length == 0)
        {
            return;
        }

        if (result < dice.MinValue || result > dice.DiceSides)
        {
            return;
        }

        if (result > sprites.Length)
        {
            return;
        }

        _diceImage.sprite = sprites[result - 1];
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}