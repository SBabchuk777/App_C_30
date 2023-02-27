using Game.Player;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Text))]
    public class MaxScoreText : MonoBehaviour
    {
        private void Awake() =>
            GetComponent<Text>().text = $"Best Score: {ScoreCounter.MaxScore}";
    }
}