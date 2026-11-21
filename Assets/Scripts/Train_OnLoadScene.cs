using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.SceneManagement;
using static DayTime;
using static Player_OnSaveLoad;
using static Town_Flags_Settings;

public class Train_OnLoadScene : MonoBehaviour
{
    DialogChoice_Anim DB;
    //int counter = 0;

    // To be saved
    string playerName = "";
    string player_TownName;
    int playerGender = -1; // 0 = girl, 1 = boys
    int[] player_trainchoices = { -1, -1, -1, -1, -1 };
    

    void ShowDialog_UI(string speaker = null)
    {
        DB.SetSpeaker(speaker);
        DB.SetText("");
        DB.ShowDialogUI();
    }
    

    void Show_ContinueIcon()
    {
        DB.Show_ContinueIcon();
    }

    void Hide_ContinueIcon()
    {
        DB.Hide_ContinueIcon();
    }

    IEnumerator WaitForMenuChoice(int defaultChoice = 0, float mindelay = 0.5f)
    {
        yield return DB.WaitForMenuChoice(defaultChoice, mindelay);
    }

    void ShowMenu(string c1, string c2 = null, string c3 = null, string c4 = null)
    {
        DB.ShowMenu(c1, c2, c3, c4);
    }

    IEnumerator TriggerAnimation(string actorName, string anim)
    {
        Debug.Log("TriggerAnimation not implemented");
        yield return new WaitForSeconds(0.1f);
    }

    IEnumerator Say(string msg, string speaker = null, bool closeOnFinish = false)
    {
        yield return DB.Say(msg, speaker, closeOnFinish);
    }

    IEnumerator Setup()
    {
        yield return TriggerAnimation("Rover", "walkInDoor");
        ShowDialog_UI("Rover");
        yield return Say("Hrm... Uh... Excuse me.\nDo you have a second?", "Rover");
        string curTime = GetCurrentTimeStr();
        string dayOfWeek = GetDayOfWeek();
        string month = GetCurrentMonth();
        string dayday = GetNumDayInMonth();
        string year = GetCurYear();
        yield return Say($"Could you help me out?\nIs it...Let's see now...\n{curTime} on {dayOfWeek}, {dayday}, {year}? ", "Rover");

        if (TownSaveFile.SaveFileExists()) { }// Keep other player's set time offset
        else { SetGameClock_Offset(TimeSpan.Zero); }

        bool confirmTime = false;
        while (!confirmTime) {
            ShowMenu("Yes", "No");
            yield return WaitForMenuChoice();
            if (DB.playerMenuChoice == 0)
            {
                yield return Say("Say, thanks! You're too kind!");
                confirmTime = true;
                break;

            }
            else {
                yield return Say("Wait, it's not?");
                yield return Say("Shit...well, err, what time is it then?");
                yield return DB.GetInput_TimeDate();
                // DB.player_dateTimeChoice should be set by this^^
                // get time offset
                SetGameClock(DB.player_dateTimeChoice); // adjust clock
                curTime = GetCurrentTimeStr();
                dayOfWeek = GetDayOfWeek();
                month = GetCurrentMonth();
                dayday = GetNumDayInMonth();
                year = GetCurYear();
                yield return Say("So, let me get this straight...");
                DateTime newTime = GetCurrentDateTime_InGame();
                yield return Say($"It's {dayOfWeek}, {month} {dayday}, {year}...\nAnd the current time is {curTime}?");
            }
        }
        // Calculate time offset
        yield return Say("So, you mind if I sit here?\nI promise I won't fall asleep,\ntumble onto you, and start\ndrooling on your shirt!");
        ShowMenu("Please!", "No way!");
        yield return WaitForMenuChoice();
        player_trainchoices[0] = DB.playerMenuChoice;

        if (player_trainchoices[0] == 0) // 'please'
        {
            yield return Say("Thanks! It sure is nice to meet friendly folks on the train.");
            yield return Say("Say, what's your name?");
        }
        else
        {
            yield return Say("It sure is nice to know there are still plenty of friendly folks on the train.\nYou know what? I'm just going to sit here anyway.");
            yield return Say("I don't even know why I'm asking this,\nbut what's your name?");
        }

        bool getNameLoop = true;
        while (getNameLoop)
        {
            yield return DB.GetStringInput("What's your name?");
            // put in player_lastStrInput
            playerName = DB.player_lastStrInput;
            
            yield return Say($"Your name is {playerName}?");
            ShowMenu("Yep!", "No! No! No!");
            DB.playerMenuChoice = -1;
            yield return WaitForMenuChoice(1);
            if (DB.playerMenuChoice == 1)
            {
                yield return Say($"Oh, ok. How do you say it?");
            }
            else if (DB.playerMenuChoice == 0) getNameLoop = false;
        }
        yield return Say($"'{playerName}'? Seriously?\nHahahahaha!");
        yield return Say($"Well, it's not like my opinion means much.\nDo YOU like your name, {playerName}?");
        ShowMenu("Isn't it cute?", "Isn't it cool?");
        yield return WaitForMenuChoice(2);
        if (DB.playerMenuChoice == 0)
        {
            yield return Say($"Yeah, it's a pretty name! Miss {playerName}!");
            playerGender = 0;
            ShowMenu("Right on!", "I'm not a girl!");
            yield return WaitForMenuChoice();
            if (DB.playerMenuChoice == 1)
            {
                playerGender = 1;
                yield return Say("You're not a girl?\nWhat?!");
                yield return Say("I mean, I knew that!\nHahah!\nI got you pretty good, didn't I?");
            }
        }
        else if (DB.playerMenuChoice == 1)
        {
            yield return Say($"Yeah, it's uh, a cool name for a guy, I guess.");
            playerGender = 1;
            ShowMenu("Yep.", "I'm not a guy!");
            yield return WaitForMenuChoice();
            if (DB.playerMenuChoice == 1)
            {
                playerGender = 0;
                yield return Say("Oh, you're not a boy?");
                yield return Say("My bad. I apologize. Really.");
            }
        }
        yield return Say("Well, changing the subject...\nWhy are you riding the train today?");
        ShowMenu("I'm moving!", "It's none of your business!");
        yield return WaitForMenuChoice();
        if (DB.playerMenuChoice == 1)
        {

            yield return Say("Chill out, man, I was just asking!");
            yield return Say("Hm...wait...could it be...you're moving?");
        }
        yield return Say("What town are you headed to?");
        while (true) { 
            yield return DB.GetStringInput("What town are you headed to?");
            player_TownName = DB.player_lastStrInput;
            yield return Say($"You're headed to {player_TownName}?");
            ShowMenu("That's right!", "No!");
            yield return WaitForMenuChoice();
            if (DB.playerMenuChoice == 1)
            {
                yield return Say("Oh, my bad.\nTell me one more time?");
            }
            else break; // return to main
        }
        yield return Say($"Oh, I've been to {player_TownName}! Lovely place!");
        yield return Say($"Next stop...{player_TownName}.\nPlease remain seated until the train doors open.", "Announcer");
        yield return Say($"Oh, I guess you gotta get off, huh?\nWas nice chatting with you, {playerName}.", "Rover");
        yield return Say($"Maybe we'll see each other again sometime?\nWould be nice...", "Rover");
        yield return Say($"Anyway, goodbye!\n Good luck with your new life in {player_TownName}!");
        Load_PullIntoTownSequence();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DB = GameObject.Find("UI_Stuff_Everything").GetComponent<DialogChoice_Anim>();
        StartCoroutine(Setup());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Load_PullIntoTownSequence()
    {
        // Maybe we just have one scene that is player town, but the first time we should load it we 
        // set the flag for town settings
        Player_OnSaveLoad.player_flag_isMoveInSequence = true;
        SceneManager.LoadScene("PlayerTown");
    }
}
