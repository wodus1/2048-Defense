using System.Linq;
using UnityEngine;

public class GlobalLibraryManager : MonoBehaviour // 글로벌 라이브러리 매니저
{
    public static GlobalLibraryManager Instance { get; private set; }

    [SerializeField] private ItemLibrary itemLibrary;

    public ItemLibrary ItemLibrary => itemLibrary;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public ItemEffect GetItemTemplate(string title)
    {
        return Instance.itemLibrary.templates.Where(x => x.Title == title).FirstOrDefault();
    }
}
