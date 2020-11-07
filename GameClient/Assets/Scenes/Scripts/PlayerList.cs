using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerList : MonoBehaviour
{

    public VerticalLayoutGroup VerticalLayoutGroup;
    public GameObject Entry;

    public void RenderPlayerList(List<string> finished, List<string> died)
    {
        RectTransform parent = VerticalLayoutGroup.GetComponent<RectTransform>();
        for (int index = 0; index < finished.Count; index++)
        {
            GameObject entry = Instantiate(Entry);
            entry.transform.Find("Position").GetComponent<Text>().text = (index + 1).ToString();
            entry.transform.Find("Time").GetComponent<Text>().text = "1.1";
            entry.transform.Find("User").GetComponent<Text>().text = finished[index];

            entry.GetComponent<RectTransform>().SetParent(parent);
        }

        for (int index = 0; index < died.Count; index++)
        {
            GameObject entry = Instantiate(Entry);
            entry.transform.Find("Position").GetComponent<Text>().text = "X";
            entry.transform.Find("Time").GetComponent<Text>().text = "---";
            entry.transform.Find("User").GetComponent<Text>().text = died[index];

            entry.GetComponent<RectTransform>().SetParent(parent);
        }
    }
}