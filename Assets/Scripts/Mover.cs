using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Mover : MonoBehaviour
{
    public Dropdown gridSize;
    public static int GridSquareSize=0;
    public static string Player1, Player2;
    public static int whoWillPlayFirst=0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void GoToGrid()
    {
        GridSquareSize = gridSize.value;
        Player1 = "Mehrab";
        Player2 = "Opi";
        whoWillPlayFirst = 0;
        SceneManager.LoadScene("GridScene");
    }
}
