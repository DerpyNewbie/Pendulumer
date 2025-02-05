using TMPro;
using UnityEngine;

namespace UI.LeaderboardLine
{
    public class LeaderboardLineElement : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text distanceText;

        public void Reset()
        {
            gameObject.SetActive(false);
        }

        public void Set(float distance)
        {
            distanceText.text = $"{distance:0.00}m";
            transform.localPosition = new Vector3(distance, 0, 0);
            gameObject.SetActive(true);
        }
    }
}