using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Liner : MonoBehaviour
{
    public GameObject pointPrefab;
    public GameObject namePrefab;
    public Sprite[] Letters;
    private Color pointPrefabColor;
    private GameObject startPoint, endPoint;
    private Color32 lineColor = new Color32(123, 227, 233, 1);
    private Color32 selectedColor = new Color32(64, 128, 192, 255);
    public LineRenderer lineRendererPrefab;
    private int[] gridSize = { 3, 5, 7, 9 };
    private float[] beginPoint = { -52.5f, -75f, -87.5f, -99f };
    public float[] incrementBy = { 35f, 30f, 25f, 22f };
    private float[] scaleSize = { 7f, 6f, 5f, 4f };
    private Dictionary<Vector3, Tuple<int, int>> pointMap;
    private bool[,,,] createdLine;
    private int squareSize;
    float beginFrom;
    float increment;
    int gridIndex;
    int lineRemaining;
    public Text currentlyPlaying, Player1Name, Player2Name, Player1Point, Player2Point;
    private string Player1 = "Mehrab";// Mover.Player1;
    private string Player2 = "Opi";// Mover.Player2;
    private int CurrentPlayer;
    private int score1 = 0, score2 = 0;
    void Start()
    {
        Player1Name.text = Player1;
        Player2Name.text = Player2;
        CurrentPlayer = Mover.whoWillPlayFirst;
        if (CurrentPlayer == 0) currentlyPlaying.text = Player1 + " is Playing Now";
        else currentlyPlaying.text = Player2 + " is Playing Now";
        startPoint = null;
        endPoint = null;
        pointMap = new Dictionary<Vector3, Tuple<int, int>>();
        pointPrefabColor = pointPrefab.GetComponent<SpriteRenderer>().color;
        gridIndex = Mover.GridSquareSize;
        squareSize = gridSize[gridIndex];
        lineRemaining = 2 * (squareSize * squareSize + squareSize);
        beginFrom = beginPoint[gridIndex];
        increment = incrementBy[gridIndex];
        createdLine = new bool[squareSize + 1, squareSize + 1, squareSize + 1, squareSize + 1];
        for (float i = beginFrom, n = 0; n <= squareSize; i += increment, n++)
        {
            for (float j = beginFrom, m = 0; m <= squareSize; j += increment, m++)
            {
                Vector3 gridPoint = new Vector3(i, j, -1f);
                GameObject point = Instantiate(pointPrefab, gridPoint, Quaternion.identity);
                pointMap[gridPoint] = Tuple.Create((int)n, (int)m);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (lineRemaining == 0)
        {
            Debug.Log("Game Over");
        }
        if (Input.GetMouseButtonDown(0))
        {
            CastRay();
        }
        if (endPoint != null)
        {
            if (validLine(ref startPoint, ref endPoint))
            {
                lineRemaining--;
                DrawLine();
                if (!validSquareCreated(startPoint, endPoint))
                {
                    CurrentPlayer ^= 1;
                }
                resetPoint(ref startPoint);
                resetPoint(ref endPoint);
            }

            else
            {
                Debug.Log("Invalid Line");
                resetPoint(ref startPoint);
                resetPoint(ref endPoint);
            }
        }
        updateText();
    }
    void updateText()
    {
        if (CurrentPlayer == 0) currentlyPlaying.text = Player1 + " is Playing Now";
        else currentlyPlaying.text = Player2 + " is Playing Now";
        Player1Point.text = score1.ToString();
        Player2Point.text = score2.ToString();
    }
    void CastRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
        if (hit.collider != null)
        {
            GameObject point = hit.collider.gameObject;
            selectPoint(point);
        }
    }
    void selectPoint(GameObject point)
    {
        if (point == startPoint) resetPoint(ref startPoint);
        else if (startPoint == null) setPoint(ref startPoint, point);
        else setPoint(ref endPoint, point);
    }
    void setPoint(ref GameObject prefab, GameObject point)
    {
        prefab = point;
        prefab.GetComponent<SpriteRenderer>().color = selectedColor;
        return;
    }
    void resetPoint(ref GameObject point)
    {
        point.GetComponent<SpriteRenderer>().color = pointPrefabColor;
        point = null;
        return;

    }
    bool validLine(ref GameObject startPoint, ref GameObject endPoint)
    {
        Vector3 startGridPoint = startPoint.transform.position;
        Vector3 endGridPoint = endPoint.transform.position;
        Tuple<int, int> point1 = pointMap[startGridPoint];
        Tuple<int, int> point2 = pointMap[endGridPoint];
        int x1 = point1.Item1, y1 = point1.Item2;
        int x2 = point2.Item1, y2 = point2.Item2;
        if (createdLine[x1, y1, x2, y2]) return false;
        if ((x1 - x2 == 0 && Math.Abs(y1 - y2) == 1) || (y1 - y2 == 0 && Math.Abs(x1 - x2) == 1))
        {
            createdLine[x1, y1, x2, y2] = true;
            createdLine[x2, y2, x1, y1] = true;
            return true;
        }
        else return false;
    }
    void DrawLine()
    {
        LineRenderer lr = Instantiate(lineRendererPrefab);
        Vector3 startPosition = new Vector3(startPoint.transform.position.x, startPoint.transform.position.y, 0);
        Vector3 endPosition = new Vector3(endPoint.transform.position.x, endPoint.transform.position.y, 0);
        lr.SetPosition(0, startPosition);
        lr.SetPosition(1, endPosition);
    }
    bool validSquareCreated(GameObject startPoint, GameObject endPoint)
    {
        Vector3 startGridPoint = startPoint.transform.position;
        Vector3 endGridPoint = endPoint.transform.position;
        Tuple<int, int> point1 = pointMap[startGridPoint];
        Tuple<int, int> point2 = pointMap[endGridPoint];
        int x1 = point1.Item1, y1 = point1.Item2;
        int x2 = point2.Item1, y2 = point2.Item2;
        if (x1 != x2) return horizontalSquare(x1, y1, x2, y2);
        else return verticalSquare(x1, y1, x2, y2);
    }
    void fillCell(int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4)
    {
        int min_x = new[] { x1, x2, x3, x4 }.Min();
        int max_x = new[] { x1, x2, x3, x4 }.Max();
        int min_y = new[] { y1, y2, y3, y4 }.Min();
        int max_y = new[] { y1, y2, y3, y4 }.Max();
        float X1 = beginFrom + increment * min_x;
        float X2 = beginFrom + increment * max_x;
        float Y1 = beginFrom + increment * min_y;
        float Y2 = beginFrom + increment * max_y;
        float mid_x = (X1+X2) / 2;
        float mid_y = (Y1 + Y2) / 2;
        GameObject letter = Instantiate(namePrefab, new Vector3(mid_x,mid_y,-1), Quaternion.identity);
        int letterID;
        if (CurrentPlayer == 0) letterID = Player1[0] - 'A';
        else letterID = Player2[0] - 'A';
        letter.GetComponent<SpriteRenderer>().sprite = Letters[letterID];
        letter.transform.localScale = new Vector3(scaleSize[gridIndex], scaleSize[gridIndex], 1);        
    }
    bool isSquare(int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4)
    {
        if (createdLine[x1, y1, x2, y2] && createdLine[x1, y1, x4, y4] && createdLine[x2, y2, x3, y3] && createdLine[x3, y3, x4, y4])
        {
            fillCell(x1, y1, x2, y2, x3, y3, x4, y4);
            IncreaseScore();
            return true;
        }
        return false;

    }
    void IncreaseScore()
    {
        if (CurrentPlayer == 0) score1++;
        else score2++;
    }
    bool horizontalSquare(int x1, int y1, int x2, int y2)
    {
        bool result = false;
        if (y2 + 1 <= squareSize && isSquare(x1, y1, x2, y2, x2, y2 + 1, x1, y1 + 1)) result|=true;
        if (y2 - 1 >= 0 && isSquare(x1, y1, x2, y2, x2, y2 - 1, x1, y1 - 1)) result|=true;
        return result;
    }
    bool verticalSquare(int x1, int y1, int x2, int y2)
    {
        bool result = false;
        if (x1 + 1 <= squareSize && isSquare(x1, y1, x1 + 1, y1, x2 + 1, y2, x2, y2)) result |= true;
        if (x1 - 1 >= 0 && isSquare(x1, y1, x1 - 1, y1, x2 - 1, y2, x2, y2)) result |= true;
        return result;
    }
}
