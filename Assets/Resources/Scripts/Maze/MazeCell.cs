using UnityEngine;

public class MazeCellInfo
{
    public int Xpos;
    public int Zpos;

    public bool Visited;

    public bool HasLeftWall;
    public bool HasRightWall;
    public bool HasTopWall;
    public bool HasDownWall;

    public MazeCellInfo(int Z, int X)
    {
        Zpos = Z;
        Xpos = X;

        HasLeftWall = true;
        HasRightWall = false;
        HasTopWall = false;
        HasDownWall = true;
    }
}

public class MazeCell : MonoBehaviour
{
    public MazeWall LeftWall;
    public MazeWall RightWall;
    public MazeWall TopWall;
    public MazeWall DownWall;

    public MazeCellInfo CellInfo;

    public Coin Coin;


    public void CreateWalls()
    {
        LeftWall.gameObject.SetActive(CellInfo.HasLeftWall);
        RightWall.gameObject.SetActive(CellInfo.HasRightWall);
        TopWall.gameObject.SetActive(CellInfo.HasTopWall);
        DownWall.gameObject.SetActive(CellInfo.HasDownWall);
    }

    public void ChangeScalesToAvoidGlitches()
    {
        Vector3 scaleChanges = new Vector3(0f, 0f, 0.001f);
        DownWall.transform.localScale -= scaleChanges;
        TopWall.transform.localScale -= scaleChanges;
    }

    public void DisableAllWalls()
    {
        DisableWall(LeftWall);
        DisableWall(RightWall);
        DisableWall(TopWall);
        DisableWall(DownWall);

        UpdateWallStates();
    }

    public void DisableWall(MazeWall wall)
    {
        if (wall.gameObject.activeSelf && !wall.OnTheEdge)
        {
            wall.gameObject.SetActive(false);
            UpdateWallStates();
        }
    }     

    public void UpdateWallStates()
    {
        CellInfo.HasLeftWall = LeftWall.gameObject.activeSelf;
        CellInfo.HasRightWall = RightWall.gameObject.activeSelf;
        CellInfo.HasTopWall = TopWall.gameObject.activeSelf;
        CellInfo.HasDownWall = DownWall.gameObject.activeSelf;
    }
}

