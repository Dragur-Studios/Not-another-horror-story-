using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Singleton;

    public GameObject Player_prefab;

    static PlayerSaveData testSave() => new PlayerSaveData { WorldPosition = Vector3.zero, WorldRotation = Quaternion.identity };

    private void Awake()
    {
        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow, new RefreshRate { numerator = 144, denominator = 1 });

        if (Singleton != null && Singleton != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        Singleton = this;
    }

    private void Start()
    {
        LoadGame();
    }

    public void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ShowCursor(bool isFree = false)
    {
        Cursor.lockState = isFree ? CursorLockMode.None : CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public void LoadGame()
    {
        var save = testSave();

        var go = Instantiate(Player_prefab);
        go.transform.position = save.WorldPosition;
        go.transform.rotation = save.WorldRotation;

        player = go.GetComponent<Player>();
        player.Initilize();

        GameCamera.Track(player.Camera.GetTrackingTarget());

    }

    public void SaveGame()
    {

    }

    Player player;
    public Player Player { get { return player; } }
}
