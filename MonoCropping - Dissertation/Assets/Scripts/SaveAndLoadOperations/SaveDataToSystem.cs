using System.IO; //Will allow to use file operations...
using System.Runtime.Serialization.Formatters.Binary; //Will allow to use binary formatters...
using UnityEngine;

//There are 3 types of saving styles as mentioned by brackeys. (But I believe there are more!)
//1. Using PlayerPrefs - Most suitable for tiny data, like highscore.
//2. Using .xml or .json - Suitable for high volume data, but are less secure as .xml and .json can be easily edited.
//3. Using Binary Formatter - Suitable for high volume data and secure since the data will be converted to binary while saving.

//A static class is a class that can't be instantiated.
public static class SaveDataToSystem
{
    //Data directory path will be different in each operating systems. Hence, manually typing in path is not recommended.
    //Therefore, we use persistentDataPath - it will fetch the path to a data directory based on the operating system.
    //Also, we can name the save data file with any name and extension we want to, as it is going to store binary data.
    static string savePath = Application.persistentDataPath + "/MCMSD.mcm"; //File name expands to: MonoCropMadnessSaveData.monocropmadness

    static BinaryFormatter formatter = new BinaryFormatter();


    //We will be saving when inside the game scene...
    public static bool SaveData(GameManager gameManager)
    {
        //BinaryFormatter formatter = new BinaryFormatter();

        //string savePath = Application.persistentDataPath + "/MCMSD.mcm"; //MonoCropMadnessSaveData.monocropmadness

        FileStream fStream = new FileStream(savePath, FileMode.Create, FileAccess.Write);
        PlayerData playerData = new PlayerData(gameManager);

        //Now to convert playerData to binary and store in file...
        formatter.Serialize(fStream, playerData);
        fStream.Close();

        if(playerData != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //We will be deleting the data from the main menu scene... But can be done from game scene as well.
    public static bool DeleteData() //https://answers.unity.com/questions/1522413/delete-a-serialized-binary-save-file.html
                                    //https://www.reddit.com/r/gamedev/comments/cyqcts/how_can_i_delete_a_binary_save_file_my_game/?sort=new
    {
        //Before deleting, double checking if file is present in the path...
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }
        else
        {
            //Debug.Log("Save File Not Found");
            return false;
        }

        //After deleting, checking if file is present in the path. If present, it is not deleted, thus return false. Else true.
        if (File.Exists(savePath))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    //We will be loading the data from the game scene... But can be done from main menu scene as well.
    public static PlayerData LoadData()
    {
        //string loadPath = Application.persistentDataPath + "/MCMSD.mcm"; //MonoCropMadnessSaveData.monocropmadness

        if (File.Exists(savePath))
        {
            //BinaryFormatter formatter = new BinaryFormatter();
            FileStream fStream = new FileStream(savePath, FileMode.Open, FileAccess.Read);

            //We have to tell it which type of data we would be needing it as - hence casting is done in the end.
            PlayerData playerData = formatter.Deserialize(fStream) as PlayerData;
            fStream.Close();

            return playerData;
        }
        else
        {
            //Debug.Log("Save File Not Found");
            return null;
        }
    }
}