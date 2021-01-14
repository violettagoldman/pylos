using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pylos;

public class BoardUI : MonoBehaviour
{
    public float CELLSIZE = 1;
    public float OFFSET = 0.5f;
    public GameObject freeSpot;
    public GameObject ballPrefab;
    public GameObject ballTmpPrefab;

    public Material toRemove;
    public Material toMove;
    
    private GameObject ballTmp = null;
    private Vector3[,] grid0 = new Vector3[1, 1];
    private Vector3[,] grid1 = new Vector3[2, 2];
    private Vector3[,] grid2 = new Vector3[3, 3];
    private Vector3[,] grid3 = new Vector3[4, 4];

    private Game game;

    private BallData ballToMove = null;
    private BallData ballToRemove = null;

    // Create a free spot object at given location
    private void CreateFreeSpot(Vector3 p, int x, int y, int stage)
    {
        GameObject tmp = Instantiate(freeSpot, p, Quaternion.identity);
        BallData d = tmp.AddComponent<BallData>() as BallData;
        d.x = x;
        d.y = y;
        d.stage = stage;
    }

    // Set the temporary transparent blue ball on the given location
    private void PlaceTempBall(BallData data) {    
        if (this.ballTmp != null)
            Destroy(this.ballTmp);
        Vector3 ballPosition = GetSpot(data).transform.position;
        ballPosition += new Vector3(0, CELLSIZE / 2, 0);
        if (data.stage != 3)
            ballPosition -= new Vector3(0, 0.3f, 0);
        this.ballTmp = Instantiate(ballTmpPrefab, ballPosition, Quaternion.identity);
        BallData d = this.ballTmp.GetComponent<BallData>();
        d.x = data.x;
        d.y = data.y;
        d.stage = data.stage;
        ShowConfirmButton();
    }

    private void ShowConfirmButton()
    {
        GameObject.Find("ButtonYes").transform.localScale = new Vector3(2, 2, 2);
    }

    private void HideConfirmationButton()
    {
        GameObject.Find("ButtonYes").transform.localScale = new Vector3(0, 0, 0);
    }

    public void ShowCancelButton()
    {
        GameObject.Find("ButtonCancel").transform.localScale = new Vector3(2, 2, 2);
    }

    private void HideCancelButton()
    {
        GameObject.Find("ButtonCancel").transform.localScale = new Vector3(0, 0, 0);
    }

    // Calculate all ball spots
    private void CalculatePositions() {
        Vector3 offset;

        // Third stage (base)
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                grid3[x, y] = new Vector3(x, 0, y);
            }
        }
        // Second stage
        offset = new Vector3(CELLSIZE / 2 * 1, OFFSET * 1, CELLSIZE / 2 * 1);
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                grid2[x, y] = new Vector3(x, 0.3f, y) + offset;
            }
        }
        // First layer
        offset = new Vector3(CELLSIZE / 2 * 2, OFFSET * 2, CELLSIZE / 2 * 2);
        for (int x = 0; x < 2; x++)
        {
            for (int y = 0; y < 2; y++)
            {
                grid1[x, y] = new Vector3(x, 0, y) + offset;
            }
        }
        // Null layer (top one)
        offset = new Vector3(CELLSIZE / 2 * 3, OFFSET * 3, CELLSIZE / 2 * 3);
        grid0[0, 0] = new Vector3(0, 0, 0) + offset;

    }

    // We remove all free spots objects (blue squares)
    private void RemoveFreeSpots()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Spot");
        for(var i = 0 ; i < gameObjects.Length ; i ++)
            Destroy(gameObjects[i]);
    }

    private void ChangeAlpha(GameObject go, float a) {
        Color c = go.GetComponent<MeshRenderer>().material.GetColor("_Color");
        c.a = a;
        go.GetComponent<MeshRenderer>().material.SetColor("_Color", c);
    }

    // We listen clicks/touches on object
    private void ListenClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit raycastHit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out raycastHit, 100f))
            {
                if (raycastHit.transform != null)
                {
                    GameObject go = raycastHit.transform.gameObject;
                    if (go.tag == "Spot") {
                        this.ballToRemove = null;
                        PlaceTempBall(go.GetComponent<BallData>());
                    }
                    if (go.tag == "Ball") {
                        BallData d = go.GetComponent<BallData>();
                        if (d.moveable) {
                            ShowCancelButton();
                            if (this.ballToMove != null)
                                ChangeAlpha(GetBall(this.ballToMove.x, this.ballToMove.y, this.ballToMove.stage), 0.5f);
                            if (this.ballTmp != null) {
                                Destroy(this.ballTmp);
                                this.ballTmp = null;
                            }
                            this.ballToMove = d;
                            ChangeAlpha(GetBall(d.x, d.y, d.stage), 1.0f);
                            ShowFreeSpots(game.EmplacementsToJump(new Position(d.stage, d.x, d.y)));
                        }
                        if (d.removeable) {
                            if (this.ballToRemove != null) {
                                ChangeAlpha(GetBall(this.ballToRemove.x, this.ballToRemove.y, this.ballToRemove.stage), 0.5f);
                            }
                            this.ballToRemove = d;
                            ChangeAlpha(GetBall(this.ballToRemove.x, this.ballToRemove.y, this.ballToRemove.stage), 1.0f);
                            ShowConfirmButton();
                        }
                    }
                }
            }
        }
    }

    void Start()
    {
        game = new Game();
        
        CalculatePositions();
        GameObject.Find("ButtonYes").GetComponent<Button>().onClick.AddListener(OnYes);
        GameObject.Find("ButtonCancel").GetComponent<Button>().onClick.AddListener(OnCancel);
        HideConfirmationButton();
        HideCancelButton();
        game.InitGame(0);
    }

    // What happens when we click the yes button
    private void OnYes()
    {
        HideCancelButton();
        BallData d;
        if (this.ballToRemove != null)
        {
            // If we clicked on a red ball to remove
            d = this.ballToRemove.GetComponent<BallData>();
            game.RemoveSquare(new Position(d.stage, d.x, d.y));
            this.ballToRemove = null;
        }
        else if (this.ballToMove != null) {
            // If we clicked on a blue ball to move
            d = this.ballTmp.GetComponent<BallData>();
            game.Jump(
                new Position(this.ballToMove.stage, this.ballToMove.x, this.ballToMove.y),
                new Position(d.stage, d.x, d.y));
            this.ballToMove = null;
        } else {
            // If we clicked on a normal spot
            d = this.ballTmp.GetComponent<BallData>();
            game.PlaceBall(new Position(d.stage, d.x, d.y));
        }
    }

    private void OnCancel() {
        if (this.ballToMove) {
            if (this.ballTmp) {
                Destroy(this.ballTmp);
                this.ballTmp = null;
            }
            ShowFreeSpots(game.EmplacementsToPlay());
            ChangeAlpha(GetBall(this.ballToMove.x, this.ballToMove.y, this.ballToMove.stage), 0.5f);
            this.ballToMove = null;
            HideCancelButton();
        }
        if (this.ballToMove == null && this.ballToRemove == null) {
            HideCancelButton();
            game.NewTurn();
        }
    }

    void Update()
    {
        ListenClick();
    }

    // Get a ball object from a position
    private GameObject GetBall(int x, int y, int stage)
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Ball");
        for(var i = 0 ; i < gameObjects.Length ; i ++)
        {
            BallData data = gameObjects[i].GetComponent<BallData>();
            if (data.x == x && data.y == y && data.stage == stage)
                return gameObjects[i];
        }
        return null;
    }

    // Get the spot (square) object fron a position
    private GameObject GetSpot(BallData d)
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Spot");
        for(var i = 0 ; i < gameObjects.Length ; i ++)
        {
            BallData data = gameObjects[i].GetComponent<BallData>();
            if (data.x == d.x && data.y == d.y && data.stage == d.stage)
                return gameObjects[i];
        }
        List<Position> list = new List<Position>();
        list.Add(new Position(d.stage, d.x, d.y));
        ShowFreeSpots(list);
        return GetSpot(d);
    }

    // After a round, reset all balls to their original colors
    public void ResetBallsColor() {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Ball")) {
            BallData d = go.GetComponent<BallData>();
            d.moveable = false;
            d.removeable = false;
            if (d.player == 1)
                go.GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(210f / 255f, 105f / 255f, 30f / 255f, 255f / 255f));
            else
                go.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.white);
        }
    }

    // Show spots where we can add/move a ball
    public void ShowFreeSpots(List<Position> positions) {
        RemoveFreeSpots();
        foreach (Position pos in positions) {
            if (pos.Stage == 3)
                CreateFreeSpot(grid3[pos.I, pos.J], pos.I, pos.J, pos.Stage);
            else if (pos.Stage == 2)
                CreateFreeSpot(grid2[pos.I, pos.J], pos.I, pos.J, pos.Stage);
            else if (pos.Stage == 1)
                CreateFreeSpot(grid1[pos.I, pos.J], pos.I, pos.J, pos.Stage);
            else if (pos.Stage == 0)
                CreateFreeSpot(grid0[pos.I, pos.J], pos.I, pos.J, pos.Stage);
        }
    }

    // Show removeable balls
    public void ShowRemoveableBalls(List<Position> positions) {
        foreach (Position pos in positions) {
            GameObject go = GetBall(pos.I, pos.J, pos.Stage);
            go.GetComponent<BallData>().removeable = true;
            // go.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
            go.GetComponent<MeshRenderer>().material = toRemove;
        }
    }

    // Show moveable balls
    public void ShowMoveableBalls(List<Position> positions) {
        foreach (Position pos in positions) {
            GameObject go = GetBall(pos.I, pos.J, pos.Stage);
            go.GetComponent<BallData>().moveable = true;
            // go.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.blue);
            go.GetComponent<MeshRenderer>().material = toMove;
        }
    }

    // Add a ball to the board, call from logic only!
    public void PlaceBall(BallData data) {
        if (this.ballTmp != null)
        {
            Destroy(this.ballTmp);
            this.ballTmp = null;
        }
        Debug.Log("add ball");
        Debug.Log(data.stage);
        Debug.Log(data.x);
        Debug.Log(data.y);
        Vector3 ballPosition = GetSpot(data).transform.position;
        ballPosition += new Vector3(0, CELLSIZE / 2, 0);
        if (data.stage != 3)
            ballPosition -= new Vector3(0, 0.3f, 0);
        GameObject tmp = Instantiate(ballPrefab, ballPosition, Quaternion.identity);
        BallData d = tmp.GetComponent<BallData>();
        d.x = data.x;
        d.y = data.y;
        d.stage = data.stage;
        d.player = game.ActualPlayer;
        if (d.player == 1)
            tmp.GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(210f / 255f, 105f / 255f, 30f / 255f, 255f / 255f));
        else
            tmp.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.white);
        HideConfirmationButton();
    }

    // Remove a ball on the board, call from logic only!
    public void RemoveBall(BallData data) {
        Destroy(GetBall(data.x, data.y, data.stage));
    }
}
