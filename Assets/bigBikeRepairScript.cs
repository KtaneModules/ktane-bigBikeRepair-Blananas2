using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;

public class bigBikeRepairScript : MonoBehaviour {

    public KMBombInfo Bomb;
    public KMAudio Audio;

    public KMSelectable[] Buttons;
    public GameObject[] StageObjects;
    public Sprite[] BikeSprites;
    public Sprite[] CircleFlaskSprites;
    public Sprite[] SquareFlaskSprites;
    public Sprite[] FireSprites;
    public SpriteRenderer[] SpriteSlots;
    public Renderer Back;

    float Hue = 0.001f;
    float Offset = 0f;
    int OffsetDirection = 0;
    int ShownBike = 0;
    int[] Bikes = {-1, -1, -1};
    int FlowchartPos = 0;
    int[] CircleFlasks = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    int FinalFlask = -1;
    int[] SquareFlasks = {-1, -1, -1, -1};

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake () {
        moduleId = moduleIdCounter++;

        foreach (KMSelectable Button in Buttons) {
            Button.OnInteract += delegate () { ButtonPress(Button); return false; };
        }
    }

    // Use this for initialization
    void Start () {
        Hue = Rnd.Range(0, 1000) * 0.001f;
        OffsetDirection = Rnd.Range(0, 4);
        StartCoroutine(BackgroundColorChanger());
        GeneratePuzzle();
    }

    void GeneratePuzzle() {
        ShownBike = 0;
        StageObjects[0].SetActive(true); StageObjects[1].SetActive(false); StageObjects[2].SetActive(false);
        SpriteSlots[13].sprite = FireSprites[0];

        Bikes[0] = Rnd.Range(0, 4);
        Bikes[1] = Rnd.Range(4, 16);
        Bikes[2] = Rnd.Range(16, 34);
        bool[][] substageOne = new bool[][] {
            new bool[] { false, false, false }, 
            new bool[] { false, true,  false }, 
            new bool[] { true,  false, true  },  
            new bool[] { true,  false, false }, 
        };
        bool[][] substageTwo = new bool[][] {
            new bool[] { true,  false, false, false, false, true,  false }, 
            new bool[] { true,  false, false, false, true,  false, false }, 
            new bool[] { true,  false, false, false, false, false, true  },  
            new bool[] { true,  false, false, true,  false, false, false }, 
            new bool[] { false, false, true,  false, false, true,  false }, 
            new bool[] { false, false, true,  false, true,  false, false }, 
            new bool[] { false, false, true,  false, false, false, true  },  
            new bool[] { false, false, true,  true,  false, false, false }, 
            new bool[] { false, true,  false, false, false, true,  false }, 
            new bool[] { false, true,  false, false, true,  false, false }, 
            new bool[] { false, true,  false, false, false, false, true  },  
            new bool[] { false, true,  false, true,  false, false, false }, 
        };
        bool[][] substageThree = new bool[][] {
            new bool[] { true,  true,  true,  false, true,  false, false, true,  false }, 
            new bool[] { false, true,  true,  true,  false, false, true,  false, false }, 
            new bool[] { false, true,  false, true,  false, true,  false, true,  false }, 
            new bool[] { true,  false, false, true,  false, false, false, true,  false }, 
            new bool[] { true,  false, false, true,  false, false, true,  false, false }, 
            new bool[] { false, true,  false, true,  false, false, true,  false, true  },  
            new bool[] { false, true,  false, true,  false, false, false, true,  false }, 
            new bool[] { true,  false, false, false, true,  false, true,  false, false }, 
            new bool[] { true,  true,  false, false, true,  false, false, true,  false }, 
            new bool[] { false, true,  false, true,  false, false, true,  false, false }, 
            new bool[] { true,  true,  false, false, true,  true,  false, true,  false }, 
            new bool[] { true,  true,  false, false, true,  false, true,  false, true  },  
            new bool[] { false, true,  false, false, true,  true,  false, true,  false }, 
            new bool[] { false, true,  false, false, true,  false, false, true,  false }, 
            new bool[] { false, true,  false, false, true,  false, true,  false, true  },  
            new bool[] { true,  false, false, true,  false, true,  true,  false, false }, 
            new bool[] { true,  false, false, true,  false, false, false, true,  true  },  
            new bool[] { false, true,  false, true,  false, false, true,  false, false }, 
        };
        Debug.LogFormat("[Big Bike Repair #{0}] Chosen bike photos: B{1}", moduleId, Bikes.Join(", B"));
        FlowchartPos = 0;
        while (FlowchartPos > -1) {
            switch (FlowchartPos) {
                case  0: FlowchartPos = ((substageOne[Bikes[0]][0]) ? 2 : 1); Debug.LogFormat("[Big Bike Repair #{0}] Did your first bike have two wheels? goes down {1} path", moduleId, ((substageOne[Bikes[0]][0]) ? "✓" : "✗")); break;
                case  1: FlowchartPos = ((substageOne[Bikes[0]][1]) ? 4 : 3); Debug.LogFormat("[Big Bike Repair #{0}] Was it missing the front wheel? goes down {1} path", moduleId, ((substageOne[Bikes[0]][1]) ? "✓" : "✗")); break;
                case  2: FlowchartPos = ((substageOne[Bikes[0]][2]) ? 5 : 4); Debug.LogFormat("[Big Bike Repair #{0}] Was it missing the handlebars? goes down {1} path", moduleId, ((substageOne[Bikes[0]][2]) ? "✓" : "✗")); break;
                case  3: FlowchartPos = ((substageTwo[Bikes[1]-4][0]) ?  7 : 6); Debug.LogFormat("[Big Bike Repair #{0}] Was the discolored part blue? goes down {1} path", moduleId, ((substageTwo[Bikes[1]-4][0]) ? "✓" : "✗")); break;
                case  4: FlowchartPos = ((substageTwo[Bikes[1]-4][1]) ?  8 : 7); Debug.LogFormat("[Big Bike Repair #{0}] Was the discolored part yellow? goes down {1} path", moduleId, ((substageTwo[Bikes[1]-4][1]) ? "✓" : "✗")); break;
                case  5: FlowchartPos = ((substageTwo[Bikes[1]-4][2]) ?  9 : 8); Debug.LogFormat("[Big Bike Repair #{0}] Was the discolored part red? goes down {1} path", moduleId, ((substageTwo[Bikes[1]-4][2]) ? "✓" : "✗")); break;
                case  6: FlowchartPos = ((substageTwo[Bikes[1]-4][3]) ? 11 : 10); Debug.LogFormat("[Big Bike Repair #{0}] Were the wheels discolored? goes down {1} path", moduleId, ((substageTwo[Bikes[1]-4][3]) ? "✓" : "✗")); break;
                case  7: FlowchartPos = ((substageTwo[Bikes[1]-4][4]) ? 12 : 11); Debug.LogFormat("[Big Bike Repair #{0}] Were the pedals discolored? goes down {1} path", moduleId, ((substageTwo[Bikes[1]-4][4]) ? "✓" : "✗")); break;
                case  8: FlowchartPos = ((substageTwo[Bikes[1]-4][5]) ? 13 : 12); Debug.LogFormat("[Big Bike Repair #{0}] Were the handlebars discolored? goes down {1} path", moduleId, ((substageTwo[Bikes[1]-4][5]) ? "✓" : "✗")); break;
                case  9: FlowchartPos = ((substageTwo[Bikes[1]-4][6]) ? 14 : 13); Debug.LogFormat("[Big Bike Repair #{0}] Was the seat discolored? goes down {1} path", moduleId, ((substageTwo[Bikes[1]-4][6]) ? "✓" : "✗")); break;
                case 10: FlowchartPos = ((substageThree[Bikes[2]-16][0]) ? 15 :-10); Debug.LogFormat("[Big Bike Repair #{0}] Was the person on your third bike wearing knee pads? goes down {1} path", moduleId, ((substageThree[Bikes[2]-16][0]) ? "✓" : "✗")); break;
                case 11: FlowchartPos = ((substageThree[Bikes[2]-16][1]) ? 16 : 15); Debug.LogFormat("[Big Bike Repair #{0}] Was the person on your third bike wearing a helmet? goes down {1} path", moduleId, ((substageThree[Bikes[2]-16][1]) ? "✓" : "✗")); break;
                case 12: FlowchartPos = ((substageThree[Bikes[2]-16][2]) ? 17 : 16); Debug.LogFormat("[Big Bike Repair #{0}] Was the person on your third bike missing a body part? goes down {1} path", moduleId, ((substageThree[Bikes[2]-16][2]) ? "✓" : "✗")); break;
                case 13: FlowchartPos = ((substageThree[Bikes[2]-16][3]) ? 18 : 17); Debug.LogFormat("[Big Bike Repair #{0}] Was the person on your third bike female? goes down {1} path", moduleId, ((substageThree[Bikes[2]-16][3]) ? "✓" : "✗")); break;
                case 14: FlowchartPos = ((substageThree[Bikes[2]-16][4]) ? -1 : 18); Debug.LogFormat("[Big Bike Repair #{0}] Was the person on your third bike male? goes down {1} path", moduleId, ((substageThree[Bikes[2]-16][4]) ? "✓" : "✗")); break;
                case 15: FlowchartPos = ((substageThree[Bikes[2]-16][5]) ? -8 : -9); Debug.LogFormat("[Big Bike Repair #{0}] Did the person wear a black shirt? goes down {1} path", moduleId, ((substageThree[Bikes[2]-16][5]) ? "✓" : "✗")); break;
                case 16: FlowchartPos = ((substageThree[Bikes[2]-16][6]) ? -6 : -7); Debug.LogFormat("[Big Bike Repair #{0}] Did the person's shirt have a number on it? goes down {1} path", moduleId, ((substageThree[Bikes[2]-16][6]) ? "✓" : "✗")); break;
                case 17: FlowchartPos = ((substageThree[Bikes[2]-16][7]) ? -4 : -5); Debug.LogFormat("[Big Bike Repair #{0}] Did the person's shirt have a letter on it? goes down {1} path", moduleId, ((substageThree[Bikes[2]-16][7]) ? "✓" : "✗")); break;
                case 18: FlowchartPos = ((substageThree[Bikes[2]-16][8]) ? -2 : -3); Debug.LogFormat("[Big Bike Repair #{0}] Did the person wear a white shirt? goes down {1} path", moduleId, ((substageThree[Bikes[2]-16][8]) ? "✓" : "✗")); break;
                default: throw new InvalidOperationException();
            }
        }
        FlowchartPos += 1;
        FlowchartPos = -FlowchartPos;
        SpriteSlots[0].sprite = BikeSprites[Bikes[0]];
        Debug.LogFormat("[Big Bike Repair #{0}] Correct circle flask: C{1}", moduleId, FlowchartPos);
        
        CircleFlasks = CircleFlasks.Shuffle();
        while (CircleFlasks[8] == FlowchartPos || CircleFlasks[9] == FlowchartPos) {
            CircleFlasks = CircleFlasks.Shuffle();
        }
        for (int f = 0; f < 8; f++) {
            SpriteSlots[f+1].sprite = CircleFlaskSprites[CircleFlasks[f]];
        }

        Reroll:
        switch (FlowchartPos%5) {
            case 0: SquareFlasks[0] = Rnd.Range(0,3); 
            for (int s = 1; s < 4; s++) {
                SquareFlasks[s] = Rnd.Range(0,21);
                while (SquareFlasks[s] < 3) {
                    SquareFlasks[s] = Rnd.Range(0,21);
                }
            }
            break;
            case 1: SquareFlasks[0] = Rnd.Range(3,9); 
            for (int s = 1; s < 4; s++) {
                SquareFlasks[s] = Rnd.Range(0,21);
                while (SquareFlasks[s] < 9 && 2 < SquareFlasks[s]) {
                    SquareFlasks[s] = Rnd.Range(0,21);
                }
            }
            break;
            case 2: SquareFlasks[0] = Rnd.Range(9,13); 
            for (int s = 1; s < 4; s++) {
                SquareFlasks[s] = Rnd.Range(0,21);
                while (SquareFlasks[s] < 13 && 8 < SquareFlasks[s]) {
                    SquareFlasks[s] = Rnd.Range(0,21);
                }
            }
            break;
            case 3: SquareFlasks[0] = Rnd.Range(13,17); 
            for (int s = 1; s < 4; s++) {
                SquareFlasks[s] = Rnd.Range(0,21);
                while (SquareFlasks[s] < 17 && 12 < SquareFlasks[s]) {
                    SquareFlasks[s] = Rnd.Range(0,21);
                }
            }
            break;
            case 4: SquareFlasks[0] = Rnd.Range(17,21); 
            for (int s = 1; s < 4; s++) {
                SquareFlasks[s] = Rnd.Range(0,21);
                while (17 < SquareFlasks[s]) {
                    SquareFlasks[s] = Rnd.Range(0,21);
                }
            }
            break;
            default: throw new InvalidOperationException();
        }
        if (SquareFlasks[1] == SquareFlasks[2] || SquareFlasks[2] == SquareFlasks[3] || SquareFlasks[1] == SquareFlasks[3]) {
            goto Reroll;
        }
        FinalFlask = SquareFlasks[0];
        Debug.LogFormat("[Big Bike Repair #{0}] Correct square flask: Q{1}", moduleId, FinalFlask);
        SquareFlasks = SquareFlasks.Shuffle();
        for (int f = 0; f < 4; f++) {
            SpriteSlots[f+9].sprite = SquareFlaskSprites[SquareFlasks[f]];
        }
    }

    void ButtonPress(KMSelectable Button) {
        Button.AddInteractionPunch();
        for (int b = 0; b < Buttons.Count(); b++) {
            if (Buttons[b] == Button) {
                if (b == 0) {
                    ShownBike += 1;
                    Audio.PlaySoundAtTransform("small", transform);
                    if (ShownBike == 3) {
                        StageObjects[0].SetActive(false); StageObjects[1].SetActive(true);
                    } else {
                        SpriteSlots[0].sprite = BikeSprites[Bikes[ShownBike]];
                    }
                } else if (b < 9) {
                    int c = b - 1;
                    if (CircleFlasks[c] == FlowchartPos) {
                        StageObjects[1].SetActive(false); StageObjects[2].SetActive(true);
                        SpriteSlots[13].sprite = FireSprites[1];
                        Audio.PlaySoundAtTransform("not small", transform);
                        Debug.LogFormat("[Big Bike Repair #{0}] Correct circle flask chosen.", moduleId);
                    } else {
                        Debug.LogFormat("[Big Bike Repair #{0}] Incorrect circle flask (C{1}) chosen, strike!", moduleId, CircleFlasks[c]);
                        GetComponent<KMBombModule>().HandleStrike();
                        GeneratePuzzle();
                    }
                } else {
                    int q = b - 9;
                    if (SquareFlasks[q] == FinalFlask) {
                        GetComponent<KMBombModule>().HandlePass();
                        StageObjects[2].SetActive(false);
                        SpriteSlots[13].sprite = FireSprites[2];
                        Audio.PlaySoundAtTransform("not small", transform);
                        Debug.LogFormat("[Big Bike Repair #{0}] Correct square flask chosen. Module solved.", moduleId);
                    } else {
                        Debug.LogFormat("[Big Bike Repair #{0}] Incorrect square flask (Q{1}) chosen, strike!", moduleId, SquareFlasks[q]);
                        GetComponent<KMBombModule>().HandleStrike();
                        GeneratePuzzle();
                    }
                }
            }
        }
    }

    IEnumerator BackgroundColorChanger () {
        while (true) {
            for (int i = 0; i < 256; i++) {
                Hue += 0.0027f;
                Hue %= 1f;
                Offset += 0.001f;
                Offset %= 0.07f;
                Back.material.color = Color.HSVToRGB(Hue, 0.5f, 0.75f);
                Back.material.mainTextureOffset = new Vector2(((OffsetDirection % 2 == 0) ? Offset : -Offset) + 0.25f, ((OffsetDirection >= 2) ? Offset : -Offset) + 0.25f);
                yield return new WaitForSeconds(0.025f);
            }
        }
    }

    //twitch plays
    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} next [Presses the next button] | !{0} potion <pos> [Chooses the potion at the specified position in reading order]";
    #pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        if (Regex.IsMatch(command, @"^\s*next\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            if (!StageObjects[0].activeSelf)
            {
                yield return "sendtochaterror The next button is not currently present!";
                yield break;
            }
            Buttons[0].OnInteract();
            yield break;
        }
        string[] parameters = command.Split(' ');
        if (Regex.IsMatch(parameters[0], @"^\s*potion\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            if (parameters.Length > 2)
                yield return "sendtochaterror Too many parameters!";
            else if (parameters.Length == 2)
            {
                if (StageObjects[1].activeSelf && parameters[1].EqualsAny("1", "2", "3", "4", "5", "6", "7", "8"))
                    Buttons[int.Parse(parameters[1])].OnInteract();
                else if (StageObjects[2].activeSelf && parameters[1].EqualsAny("1", "2", "3", "4"))
                    Buttons[int.Parse(parameters[1]) + 8].OnInteract();
                else if (StageObjects[0].activeSelf)
                    yield return "sendtochaterror There are not any potions currently present!";
                else
                    yield return "sendtochaterror!f The specified position '" + parameters[1] + "' is invalid!";
            }
            else if (parameters.Length == 1)
                yield return "sendtochaterror Please specify the position of the potion you wish to choose!";
        }
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        while (StageObjects[0].activeSelf)
        {
            Buttons[0].OnInteract();
            yield return new WaitForSeconds(.1f);
        }
        if (StageObjects[1].activeSelf)
        {
            Buttons[Array.IndexOf(CircleFlasks, FlowchartPos) + 1].OnInteract();
            yield return new WaitForSeconds(.1f);
        }
        Buttons[Array.IndexOf(SquareFlasks, FinalFlask) + 9].OnInteract();
    }
}
