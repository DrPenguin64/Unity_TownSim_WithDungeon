using System;
using System.Collections;
using System.Linq;
using System.Net.Mime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class DialogChoice_Anim : MonoBehaviour
{

    // Related to current choices
    public DateTime player_dateTimeChoice = DateTime.MinValue;
    public string player_lastStrInput = "";
    public int playerMenuChoice = -1;
    string[] currentChoices_str;
    int currentChoices_Len = -1;


    bool flag_inputString_submitted = false;
    bool flags_ArrowKeyMenuCheck = false; // whether to run update 
    bool flag_validtimeDateInput_Submitted = false;
    //bool timeDate_submitRequested = false; // whether timedate_submit button was pressed

    // UI Object references
    private GameObject dialogpanel_root;
    private GameObject continueIcon;
    private TextMeshProUGUI textObject;
    private TextMeshProUGUI speaker_textObject;
    private GameObject choiceMenuPanel;
    private TextMeshProUGUI choice1;
    private TextMeshProUGUI choice2;
    private TextMeshProUGUI choice3;
    private TextMeshProUGUI choice4;
    private TextMeshProUGUI[] choicesArr;

    // Input field ui
    private GameObject inputStringField_Root;
    private TextMeshProUGUI inputStringField_Prompt;
    private TMP_InputField inputStringField_InputField;

    // Popup ui
    private GameObject timedateError_popup_root;
    private TextMeshProUGUI timedateError_textField;

    // Input datetime ui
    private GameObject inputTimeDate_Root;
    private TMP_InputField inputTimeDate_Year;
    private TMP_Dropdown inputTimeDate_Month;
    private TMP_InputField inputTimeDate_Day;
    private TMP_InputField inputTimeDate_Hour;
    private TMP_InputField inputTimeDate_Minute;
    private TMP_Dropdown inputTimeDate_AMPM;

    private Coroutine textAnimObj; // stores current anim, so you can stop it

    private Coroutine popup_coroutine;

    void Init_Datetime_GUI()
    {
        inputTimeDate_Month.ClearOptions();
        inputTimeDate_Month.AddOptions(DayTime.MONTHS.ToList());
        HidePopup_TimeDateError();
    }

    private void GetUI_GameobjectRefs()
    {
        //Timedate
        inputTimeDate_Root = GameObject.Find("Canvas_InputTimeDate");
        inputTimeDate_Year = GameObject.Find("inputYear").GetComponent<TMP_InputField>();
        inputTimeDate_Month = GameObject.Find("inputMonth").GetComponent<TMP_Dropdown>();
        inputTimeDate_Day = GameObject.Find("inputDay").GetComponent<TMP_InputField>();
        inputTimeDate_Hour = GameObject.Find("inputHour").GetComponent<TMP_InputField>();
        inputTimeDate_Minute = GameObject.Find("inputMinute").GetComponent<TMP_InputField>();
        inputTimeDate_AMPM = GameObject.Find("input_AMPM").GetComponent<TMP_Dropdown>();

        //Popup
        timedateError_popup_root = inputTimeDate_Root.transform.Find("inputTimeDate_errorpopup").gameObject;
        timedateError_textField = timedateError_popup_root.transform.Find("popup_maintext").GetComponent<TextMeshProUGUI>();

        //Inputstring
        inputStringField_Root = GameObject.Find("Canvas_InputTextField");
        inputStringField_Prompt = GameObject.Find("inputStringField_Prompt").GetComponent<TextMeshProUGUI>();
        inputStringField_InputField = GameObject.Find("inputString_TextField").GetComponent<TMP_InputField>();

        //Dialog+choices
        dialogpanel_root = GameObject.Find("dialogBox_root");
        continueIcon = dialogpanel_root.transform.Find("continueImg").gameObject;
        textObject = GameObject.Find("dialogBody").GetComponent<TextMeshProUGUI>();
        speaker_textObject = GameObject.Find("speaker_textObject").GetComponent<TextMeshProUGUI>();
        choiceMenuPanel = GameObject.Find("ChoiceMenuPanel");
        choice1 = choiceMenuPanel.transform.Find("choice1").GetComponent<TextMeshProUGUI>();
        choice2 = choiceMenuPanel.transform.Find("choice2").GetComponent<TextMeshProUGUI>();
        choice3 = choiceMenuPanel.transform.Find("choice3").GetComponent<TextMeshProUGUI>();
        choice4 = choiceMenuPanel.transform.Find("choice4").GetComponent<TextMeshProUGUI>();

        choicesArr = new TextMeshProUGUI[] { choice1, choice2, choice3, choice4 };// ez access

        foreach (TextMeshProUGUI s in choicesArr)
        {
            if (s == null)
            {
                Debug.LogError("DialogChoice_Anim Error: couldn't find one choiceTextMeshPro");
            }
        }
    }

    public void Show_ContinueIcon()
    {
        continueIcon.SetActive(true);
    }

    public void Hide_ContinueIcon()
    {
        continueIcon.SetActive(false);
    }

    public void ShowDialogUI()
    {
        dialogpanel_root.SetActive(true);
    }

    public void HideDialogUI()
    {
        dialogpanel_root.SetActive(false);
    }

    public void HideChoicesUI()
    {
        choice1.gameObject.SetActive(false);
        choice2.gameObject.SetActive(false);
        choice3.gameObject.SetActive(false);
        choice4.gameObject.SetActive(false);
        choiceMenuPanel.SetActive(false);
    }

    private void Awake()
    {
        // get the references
        GetUI_GameobjectRefs();
        Init_Datetime_GUI();

        // Hide choices by default
        HideChoicesUI();
        HideDialogUI();
        Hide_GetString_UI();
        HidePopup_TimeDateError();
        Hide_GetInputTimeDate_UI();
    }

    public void HideAll_UI()
    {
        HideChoicesUI();
        HideDialogUI();
        Hide_GetString_UI();
        HidePopup_TimeDateError();
        Hide_GetInputTimeDate_UI();
    }

    public IEnumerator Say(string msg, string speaker = null, bool closeOnFinish = false)
    {
        Hide_ContinueIcon();

        float newline_Delay = 0.15f;
        float commaDelay = 0.15f;
        float periodDelay = 0.23f;
        float charDelay = 0.08f;

        if (speaker != null) SetSpeaker(speaker);

        bool keyDown_During_This = false;
        string[] lines = msg.Split("\n");
        string my_Buffer = "";
        // Type msg
        for (int i = 0; i < lines.Length; i++)
        {
            string myLine = lines[i];
            for (int j = 0; j < myLine.Length; j++)
            {
                if (Input.GetKeyDown(Player_ControlSettings.interactUse)) keyDown_During_This = true;

                my_Buffer += myLine[j];
                SetText(my_Buffer);

                newline_Delay = 0.15f;
                commaDelay = 0.15f;
                periodDelay = 0.23f;
                charDelay = 0.05f;
                if (keyDown_During_This && (Input.GetKey(Player_ControlSettings.interactUse) || Input.GetKey(Player_ControlSettings.cancel)))
                {
                    newline_Delay /= 3;
                    commaDelay /= 3;
                    periodDelay /= 3;
                    charDelay /= 3;
                }
                // Choose delay based on character
                if (j == myLine.Length - 1 && i == lines.Length - 1)
                {
                    yield return new WaitForSeconds(charDelay);
                }
                else
                {
                    switch (myLine[j])
                    {
                        case ',':
                            yield return new WaitForSeconds(commaDelay);
                            break;
                        case '.':
                        case '?':
                        case '!':
                            yield return new WaitForSeconds(periodDelay);
                            break;
                        default:
                            yield return new WaitForSeconds(charDelay);
                            break;
                    }
                }

            }//newline 
            if (i != lines.Length - 1)
            {
                my_Buffer += "\n";
                yield return new WaitForSeconds(newline_Delay);
            }
        }

        // Wait
        yield return new WaitForEndOfFrame();
        Show_ContinueIcon();
        yield return WaitForConfirm();
        Hide_ContinueIcon();
        if (closeOnFinish)
        {
            SetText("null");
            SetSpeaker("???");
            HideDialogUI();
        }
    }

    private void Cursor_SelectChoice(int newChoice, bool loop = false)
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

    private void Start_ArrowKeys_SelectChoice()
    {
        flags_ArrowKeyMenuCheck = true;
    }

    private void ArrowKeys_SelectChoice() // runs in Update();
    {
        int target = playerMenuChoice;
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow)) target += 1;
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.UpArrow)) target -= 1;
        Cursor_SelectChoice(target);


        // submit? HANDLED in WaitForMenuChoice
        //if (Input.GetKeyDown(Player_ControlSettings.submit))

    }

    private void Stop_ArrowKeys_SelectChoice()
    {
        flags_ArrowKeyMenuCheck = false;
    }

    private void Show_GetInputTimeDate_UI()
    {
        flag_validtimeDateInput_Submitted = false;
        inputTimeDate_Year.characterLimit = 4;
        inputTimeDate_Year.characterValidation = TMP_InputField.CharacterValidation.Digit;
        inputTimeDate_Day.characterLimit = 2;
        inputTimeDate_Day.characterValidation = TMP_InputField.CharacterValidation.Digit;

        inputTimeDate_Hour.characterLimit = 2;
        inputTimeDate_Hour.characterValidation = TMP_InputField.CharacterValidation.Digit;
        inputTimeDate_Minute.characterLimit = 2;
        inputTimeDate_Minute.characterValidation = TMP_InputField.CharacterValidation.Digit;

        // show
        inputTimeDate_Root.SetActive(true);
    }

    private void Hide_GetInputTimeDate_UI()
    {
        flag_validtimeDateInput_Submitted = false;
        // Clear inputs
        inputTimeDate_Root.SetActive(false);
    }

    public IEnumerator GetInput_TimeDate()
    {
        flag_validtimeDateInput_Submitted = false;
        Show_GetInputTimeDate_UI();//
        yield return new WaitUntil(() => flag_validtimeDateInput_Submitted);

        // Set output
        // Extract fields
        int year = int.Parse(inputTimeDate_Year.text);
        int month = inputTimeDate_Month.value + 1;
        int day = int.Parse(inputTimeDate_Day.text);

        int hour = int.Parse(inputTimeDate_Hour.text);
        if (inputTimeDate_AMPM.value == 1 && hour != 12) hour += 12; // If pm
        else if (inputTimeDate_AMPM.value == 0 && hour == 12) hour = 0; // if 12am
        int minute = int.Parse(inputTimeDate_Minute.text);

        // Set datetime object
        player_dateTimeChoice = new DateTime(year, month, day, hour, minute, 0);
        // Done
        Hide_GetInputTimeDate_UI();
    }

    public void OnTimeDateInput_SubmitButton_Clicked()
    {
        // Check if input submitted is valid
        if (TimeDayYearMonth_IsValid()) flag_validtimeDateInput_Submitted = true; // set the flag
        else {
            if (popup_coroutine != null) StopCoroutine(popup_coroutine);
            popup_coroutine = StartCoroutine(ShowPopup_TimeDateError("Invalid input.", 3));
        }

    }

    public IEnumerator ShowPopup_TimeDateError(string msg, float timeout = 2.0f)
    {
        timedateError_popup_root.SetActive(true);
        timedateError_textField.text = msg;
        yield return new WaitForSeconds(timeout);
        timedateError_popup_root.SetActive(false);
    }

    private void HidePopup_TimeDateError()
    {
        timedateError_textField.text = "null";
        timedateError_popup_root.SetActive(false);
    }

    public bool TimeDayYearMonth_IsValid()
    {
        // Extract fields
        int year = int.Parse(inputTimeDate_Year.text);
        int month = inputTimeDate_Month.value + 1;
        int day = int.Parse(inputTimeDate_Day.text);

        int hour = int.Parse(inputTimeDate_Hour.text);
        if (inputTimeDate_AMPM.value == 1 && hour != 12) hour += 12; // If pm
        else if (inputTimeDate_AMPM.value == 0 && hour == 12) hour = 0; // if 12am
        int minute = int.Parse(inputTimeDate_Minute.text);

        if (yearIsValid(year) && dayIsValid(day, month, year) && hourIsValid(hour) && minuteIsValid(minute)) return true;
        // print info about which is the problem
        if (!yearIsValid(year)) print("Year is invalid");
        else if (!dayIsValid(day, month, year)) print("Day is invalid for given month/year");
        else if (!hourIsValid(hour)) print("Hour is invalid");
        else if (!minuteIsValid(minute)) print("Minute is invalid");
        return false;
    }

    // Time validation methods
    private bool yearIsValid(int year) {
        
        if (year >= 1950 && year <= 2080) return true;
        else return false;
    }
    private bool dayIsValid(int day, int month, int year)
    {
        return day >= 1 && day <= DateTime.DaysInMonth(year, month);
    }

    private bool hourIsValid(int hour) { return hour >= 0 && hour <= 23; }

    private bool minuteIsValid(int minute) { return minute >= 0 && minute <= 59;}

    //-- String input
    public void Show_GetString_UI(string prompt, int maxLen=0)
    {
        inputStringField_Prompt.text = prompt;
        // Set character limit
        inputStringField_InputField.characterLimit = maxLen;
        inputStringField_InputField.text = ""; // clear default input text

        inputStringField_Root.SetActive(true);
    }

    public void Hide_GetString_UI(bool clearValues = true)
    {
        inputStringField_Prompt.text = "default prompt";
        inputStringField_InputField.text = "";
        inputStringField_InputField.characterLimit = 0;

        inputStringField_Root.SetActive(false);
    }

    public IEnumerator GetStringInput(string prompt, int maxLen = 0)
    {
        //inputStringField_Prompt.text = 
        Show_GetString_UI(prompt, maxLen);
        // Wait for submit
        flag_inputString_submitted = false;
        yield return new WaitUntil(() => flag_inputString_submitted);
        player_lastStrInput = inputStringField_InputField.text;
        Hide_GetString_UI();
        // Hide
    }

    public void OnStringInput_SubmitButton_Clicked()
    {
        flag_inputString_submitted = true;
    }



    public IEnumerator WaitForMenuChoice(int defaultChoice = 0, float mindelay = 0.5f)
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

    public void ShowMenu(string c1, string c2 = null, string c3 = null, string c4 = null)
    {
        currentChoices_str = new string[] { c1, c2, c3, c4 }; // reset it
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

    public IEnumerator WaitForConfirm()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(Player_ControlSettings.submit) || Input.GetKeyDown(Player_ControlSettings.interactUse));
        // todo: play sound effect?
    }

    public void TypeText(string txt, float waitTime)
    {

    }


    public void SetText_Anim(float waitAmount, params string[] variants)
    {
        if (textAnimObj != null) StopCoroutine(textAnimObj);
        textAnimObj = StartCoroutine(_AnimateText(variants, waitAmount, 0));
    }

    public IEnumerator _AnimateText(string[] variants, float waitAmount, int i = 0)
    {
        if (i >= variants.Length) i = 0;
        SetText(variants[i]);
        yield return new WaitForSeconds(waitAmount);
        //_AnimateText(variants, waitAmount, i + 1);
        yield return _AnimateText(variants, waitAmount, i + 1);
    }

    public void SetText(string what)
    {
        textObject.text = what;
    }

    public void SetSpeaker(string speaker)
    {
        speaker_textObject.text = speaker;
    }

    // Update is called once per frame
    private void Update()
    {
        if (flags_ArrowKeyMenuCheck)
        {
            ArrowKeys_SelectChoice();
        }
    }
}
