using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game_OnLoad : MonoBehaviour
{
    public bool saveFound = false;

    // Settings

    // Related to current choices
    int playerMenuChoice = -1;
    string[] currentChoices_str;
    int currentChoices_Len = -1;

    bool flags_ArrowKeyMenuCheck = false; // whether to run update 

    // UI Object references
    private TextMeshProUGUI textObject;
    private GameObject choiceMenuPanel;
    private TextMeshProUGUI choice1;
    private TextMeshProUGUI choice2;
    private TextMeshProUGUI choice3;
    private TextMeshProUGUI choice4;
    private TextMeshProUGUI[] choicesArr;

    Coroutine textAnimObj; // stores current anim, so you can stop it


    void GetUI_GameobjectRefs()
    {
        textObject = GameObject.Find("DebugDisplay").GetComponent<TextMeshProUGUI>();
        choiceMenuPanel = GameObject.Find("ChoiceMenuPanel");
        choice1 = choiceMenuPanel.transform.Find("choice1").GetComponent<TextMeshProUGUI>();
        choice2 = choiceMenuPanel.transform.Find("choice2").GetComponent<TextMeshProUGUI>();
        choice3 = choiceMenuPanel.transform.Find("choice3").GetComponent<TextMeshProUGUI>();
        choice4 = choiceMenuPanel.transform.Find("choice4").GetComponent<TextMeshProUGUI>();

        choicesArr = new TextMeshProUGUI[] { choice1, choice2, choice3, choice4 };// ez access
    }

    void HideChoicesUI()
    {
        choice1.gameObject.SetActive(false);
        choice2.gameObject.SetActive(false);
        choice3.gameObject.SetActive(false);
        choice4.gameObject.SetActive(false);
        choiceMenuPanel.SetActive(false);
    }

    private void Awake()

    {
        G_Registry.CreateRegistry(); //
        // get the references
        GetUI_GameobjectRefs();
        // Hide choices by default
        HideChoicesUI();
    }

    void Start()
    {
        StartCoroutine(Setup());
    }

    IEnumerator Setup()
    {

        Setup_Registries();

        // Check for saved games
        SetText("Looking for saved games...");
        SetText_Anim(0.3f, "Looking for saved games.", "Looking for saved games..", "Looking for saved games...");

        bool force_createNewGame = false;
        bool continueLoop = true;
        yield return LoadSave(); // Wait for save
        StopCoroutine(textAnimObj);
        if (saveFound) {
            SetText("Save file found!");
            yield return WaitForConfirm();
            while (continueLoop)
            {
                SetText("Continue from last save, or start a new game?");
                ShowMenu("Continue", "New Game");
                yield return WaitForMenuChoice();
                if (playerMenuChoice == 0)
                {
                    SetText("Press enter to continue to your town...");
                    yield return WaitForConfirm();
                    LoadSavedGame_StartScene();
                }
                else
                {
                    SetText("Are you sure you want to start a new game?");
                    ShowMenu("No", "Yes");
                    yield return WaitForMenuChoice();
                    if (playerMenuChoice == 1) { force_createNewGame = true; continueLoop = false; }
                }
            }
        }


        if (!saveFound || force_createNewGame)
        {
            SetText("No save file found.");
            yield return WaitForConfirm();
            SetText("Create a new save file?");
            ShowMenu("Yes", "No");
            yield return WaitForMenuChoice();
            Debug.Log("You chose" + choicesArr[playerMenuChoice]);
            HideChoicesUI();
            
            if (playerMenuChoice == 0) // chose yes
            {
                SetText("Creating a new save file...");
                SetText_Anim(0.3f, "Creating a new save file.", "Creating a new save file..", "Creating a new save file...");
                yield return CreateNewSave();
                StopCoroutine(textAnimObj);
                SetText("Save file created.");
                yield return WaitForConfirm();
                SetText("Have fun in the game!");
                yield return WaitForConfirm();
                LoadTrainSequence();
            }
            else // player chose no
            {

            }
        }

    }

    
    // Loads the required scene and switches to it
    void LoadTrainSequence()
    {
        SceneManager.LoadScene("TrainSequence");
    }
    
    void LoadSavedGame_StartScene()
    {
        SceneManager.LoadScene("PlayerTown");
    }

    void Cursor_SelectChoice(int newChoice, bool loop = false)
    {
        // Validate/fix
        if (newChoice < 0)
        {
            if (loop)
            {
                newChoice = currentChoices_Len - 1;
            }
            else newChoice = playerMenuChoice; // keep the same as before
        }
        if (newChoice >= currentChoices_Len)
        {
            if (loop) playerMenuChoice = 0;
            else newChoice = playerMenuChoice; // keep same as before
        }

        // Update display
        for (int j = 0; j < currentChoices_Len; j++)
        {
            if (j == newChoice) choicesArr[j].text = "> " + currentChoices_str[j];
            else choicesArr[j].text = "  " + currentChoices_str[j];
        }

        // Set to current choice
        playerMenuChoice = newChoice;
    }

    void Start_ArrowKeys_SelectChoice()
    {
        flags_ArrowKeyMenuCheck = true;
    }
    
    void ArrowKeys_SelectChoice() // runs in Update();
    {
        int target = playerMenuChoice;
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow)) target += 1;
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.UpArrow)) target -= 1;
        Cursor_SelectChoice(target);


        // submit? HANDLED in WaitForMenuChoice
        //if (Input.GetKeyDown(Player_ControlSettings.submit))
        
    }

    void Stop_ArrowKeys_SelectChoice()
    {
        flags_ArrowKeyMenuCheck = false;
    }

    IEnumerator WaitForMenuChoice(int defaultChoice = 0, float mindelay = 0.5f)
    {
        Cursor_SelectChoice(defaultChoice);
        // Start coruotine
        Start_ArrowKeys_SelectChoice();
        // Wait for update....
        // Wait
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(mindelay);
        yield return new WaitUntil(() => Input.GetKeyDown(Player_ControlSettings.submit) || Input.GetKeyDown(Player_ControlSettings.interactUse));
        //yield return new WaitUntil(() => Input.GetKeyDown(Player_ControlSettings.submit)); // wait until submit
        Stop_ArrowKeys_SelectChoice();
        HideChoicesUI();
    }

    void ShowMenu(string c1, string c2 = null, string c3 = null, string c4 = null)
    {
        currentChoices_str = new string[]{ c1, c2, c3, c4}; // reset it
        currentChoices_Len = 0;
        if (c1 != null)
        {
            choice1.text = c1;
            choice1.gameObject.SetActive(true);
            currentChoices_Len = 1;
        }
        if (c2 != null)
        {
            choice2.text = c2;
            choice2.gameObject.SetActive(true);
            currentChoices_Len = 2;
        }
        if (c3 != null)
        {
            choice3.text = c3;
            choice3.gameObject.SetActive(true);
            currentChoices_Len = 3;
        }
        if (c4 != null)
        {
            choice4.text = c4;
            choice4.gameObject.SetActive(true);
            currentChoices_Len = 4;
        }
        choiceMenuPanel.SetActive(true); // show it
    }

    IEnumerator WaitForConfirm()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(Player_ControlSettings.submit) || Input.GetKeyDown(Player_ControlSettings.interactUse));
        // todo: play sound effect?
    }

    IEnumerator LoadSave()
    {
        yield return new WaitForSeconds(1.0f);
    }

    IEnumerator CreateNewSave()
    {
        yield return new WaitForSeconds(2.0f);
    }

    void TypeText(string txt, float waitTime)
    {

    }


    void SetText_Anim(float waitAmount, params string[] variants)
    {
        if (textAnimObj != null) StopCoroutine(textAnimObj);
        textAnimObj = StartCoroutine(_AnimateText(variants, waitAmount, 0));
    }

    IEnumerator _AnimateText(string[] variants, float waitAmount, int i = 0)
    {
        if (i >= variants.Length) i = 0;
        SetText(variants[i]);
        yield return new WaitForSeconds(waitAmount);
        //_AnimateText(variants, waitAmount, i + 1);
        yield return _AnimateText(variants, waitAmount, i + 1);
    }

    bool TryLoadSave()
    {
        return false; // todo
    }

    public void SetText(string what)
    {
        textObject.text = what;
    }

    // Update is called once per frame
    void Update()
    {
        if (flags_ArrowKeyMenuCheck)
        {
            ArrowKeys_SelectChoice();
        }
    }
}
