using UnityEngine;
using System;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine.UI;


// This scripts handles signin with google play services
public class PlayGames : MonoBehaviour
{
    public static PlayGamesPlatform platform;
    public int totalCoins;




    void Start()
    {
      

        if (platform == null)
        {
            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();
            PlayGamesPlatform.InitializeInstance(config);
            PlayGamesPlatform.DebugLogEnabled = true;
            platform = PlayGamesPlatform.Activate();
        }

        PlayGamesPlatform.Instance.Authenticate(SignInInteractivity.CanPromptOnce, SignInResult);

  
    }

    private string GetSaveString()
    {
        string r = "";
        r += PlayerPrefs.GetFloat("HiScore").ToString("0");
        r += "|";
        r += totalCoins.ToString();

        return r;
    }

    private void LoadSaveString(string save)
    {
        ShowAndroidToastMessage("Loading Cloud");
        string[] data = save.Split('|');
        PlayerPrefs.SetFloat("HiScore", float.Parse(data[0]));
   

        totalCoins = int.Parse(data[1]);

        GameManager.Instance.totalCoinAmountText.text = "Coins "+ totalCoins.ToString();



    }

    private void SignInResult(SignInStatus status)
    {
        ShowAndroidToastMessage(status.ToString());
        if(status.ToString() == "Success")
        {
            OpenSave(false);
        }
        
    }

    private bool isSaving = false;

    // Cloud Saving
    public void OpenSave(bool saving)
    {
     

        if (Social.localUser.authenticated)
        {
            isSaving = saving;
            ((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution("PinguRun", GooglePlayGames.BasicApi.DataSource.ReadCacheOrNetwork,
                ConflictResolutionStrategy.UseLongestPlaytime, SaveGameOpened);

        }
    }

    private void SaveGameOpened(SavedGameRequestStatus status, ISavedGameMetadata meta)
    {
       
        if(status == SavedGameRequestStatus.Success)
        {
            
            if (isSaving) // Writing
            {
                byte[] data = System.Text.ASCIIEncoding.ASCII.GetBytes(GetSaveString());
                SavedGameMetadataUpdate update = new SavedGameMetadataUpdate.Builder().WithUpdatedDescription("Saved at " + DateTime.Now.ToString()).Build();


                ((PlayGamesPlatform)Social.Active).SavedGame.CommitUpdate(meta, update, data, SaveUpdate);
            }
            else //Reading
            {
               

                ((PlayGamesPlatform)Social.Active).SavedGame.ReadBinaryData(meta, SaveRead);
            }
        }
       
    }

    // Load
    private void SaveRead(SavedGameRequestStatus status, byte[] data)
    {
        if(status == SavedGameRequestStatus.Success)
        {
            string saveData = System.Text.ASCIIEncoding.ASCII.GetString(data);
            LoadSaveString(saveData);
            Debug.Log(saveData);
        }
    }

    // Success Save
    private void SaveUpdate(SavedGameRequestStatus status, ISavedGameMetadata meta)
    {
       
    }


    public void ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity, message, 0);
                toastObject.Call("show");
            }));
        }
    }


}