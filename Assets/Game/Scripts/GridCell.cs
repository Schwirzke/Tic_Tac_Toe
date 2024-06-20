using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GridCell : MonoBehaviour
{
    [SerializeField] private TextMeshPro textObject;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void Reset()
    {
        textObject.text = " ";
    }

    public string GetSymbol()
    {
        return textObject.text;
    }

    public void SetSymbol(string newSymbol)
    {
        textObject.text = newSymbol;

        textObject.color = newSymbol.Equals("X") ? Color.red : Color.white;
    }

    public bool isBlank()
    {
        return textObject.text.Equals(" ");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
