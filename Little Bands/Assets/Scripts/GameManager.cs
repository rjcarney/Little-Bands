using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public GameObject SongListPage;
    public GameObject InstrumentSelect;
    public GameObject RecordPage;

    private SongItem SelectedSong;
    private string SelectedInstrument;

    // Start is called before the first frame update
    void Awake()
    {
        if(instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        SongListPage.SetActive(true);
        InstrumentSelect.SetActive(false);
        RecordPage.SetActive(false);
        SelectedSong = null;
    }

    public void selectSong(GameObject SongListItem) {
        SelectedSong = SongListItem.GetComponent<SongItem>();
        SongListPage.SetActive(false);
        InstrumentSelect.SetActive(true);
    }

    public void selectInstrument(string instrument) {
        SelectedInstrument = instrument;
        InstrumentSelect.SetActive(false);
        RecordPage.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
