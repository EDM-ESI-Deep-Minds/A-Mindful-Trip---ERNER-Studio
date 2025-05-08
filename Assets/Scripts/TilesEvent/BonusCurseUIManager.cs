using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using Unity.Collections;
using System.Collections;

public class BonusCurseUIManager : NetworkBehaviour
{
    public static BonusCurseUIManager Instance;
    [SerializeField] private GameObject MessageUIPrefab;
    [SerializeField] private GameObject HelpRequest;
    private GameObject spawnedUI;

    public Transform canvasTransform;

    [System.Serializable]
    public class EffectData
    {
        public string mainText;
        public string[] flavorTexts;
    }

    public Dictionary<string, EffectData> curses = new Dictionary<string, EffectData>()
    {
        {
            "remove_heart",
            new EffectData {
                mainText = "You lost a heart.",
                flavorTexts = new string[] {
                    "The darkness feeds…",
                    "Your heart shatters like glass.",
                    "One step closer to doom.",
                    "A shadow takes a piece of your soul.",
                    "A lightning strike drains your strength."
                }
            }
        },
        {
            "remove_credit",
            new EffectData {
                mainText = "You lost some credits.",
                flavorTexts = new string[] {
                    "Your pockets feel lighter.",
                    "Greedy spirits took your coins.",
                    "Oops... your wallet slipped into another dimension.",
                    "You paid the price for curiosity."
                }
            }
        },
        {
            "remove_item",
            new EffectData {
                mainText = "An item was lost.",
                flavorTexts = new string[] {
                    "Your prized possession is gone!",
                    "Something vanished from your bag…",
                    "The curse devours your gear.",
                    "You hear a faint laugh in the wind."
                }
            }
        },
        {
            "mute",
            new EffectData {
                mainText = "You've been muted.",
                flavorTexts = new string[] {
                    "Your voice fades into silence.",
                    "The curse seals your lips.",
                    "You try to speak, but nothing comes out.",
                    "Only your eyes can scream now."
                }
            }
        },
        {
            "lock_inventory",
            new EffectData {
                mainText = "Your inventory is locked.",
                flavorTexts = new string[] {
                    "Your belongings are sealed away.",
                    "No touching! The curse forbids it.",
                    "A rusty lock binds your bag shut.",
                    "No item shall leave it now."
                }
            }
        },
        {
            "reposition",
            new EffectData {
                mainText = "You moved backwards.",
                flavorTexts = new string[] {
                    "The ground beneath you crumbles!",
                    "Time rewinds... poorly.",
                    "A mysterious force pulls you back.",
                    "Was that really the right direction?"
                }
            }
        },
    };

    public Dictionary<string, EffectData> bonuses = new Dictionary<string, EffectData>()
    {
        {
            "credit_bonus",
            new EffectData {
                mainText = "You gained some credits!",
                flavorTexts = new string[] {
                    "Sweet coinage!",
                    "Gold glimmers in your path.",
                    "Jackpot!",
                    "A reward for your journey."
                }
            }
        },
        {
            "rare_credit_bonus",
            new EffectData {
                mainText = "You gained a rare credit bonus!",
                flavorTexts = new string[] {
                    "A fortune smiles upon you.",
                    "Your luck is unmatched!",
                    "The stars align in your favor.",
                    "You just hit the jackpot of fate!"
                }
            }
        },
        {
            "next_reward_boost",
            new EffectData {
                mainText = "Your next reward will be stronger.",
                flavorTexts = new string[] {
                    "Something good is brewing...",
                    "Your path glows with promise.",
                    "Patience is rewarded generously",
                    "Bonus mode activated!",
                    "Good things come to those who wait."
                }
            }
        },
        {
            "add_item",
            new EffectData {
                mainText = "You received a new item.",
                flavorTexts = new string[] {
                    "A gift for the worthy.",
                    "Your hands grasp a mysterious object.",
                    "Treasure appears from thin air!",
                    "The game favors you... for now."
                }
            }
        },
        {
            "add_heart",
            new EffectData {
                mainText = "You gained a heart!",
                flavorTexts = new string[] {
                    "You feel stronger.",
                    "A warm glow fills you.",
                    "Life flows back into you.",
                    "A second chance… use it wisely."
                }
            }
        },
    };

    public Dictionary<string, EffectData> rest = new Dictionary<string, EffectData>()
    {
        {
            "rest",
            new EffectData
            {
                 mainText = "A Moment to Rest",
                flavorTexts = new string[]
                {
                    "You find a quiet spot to catch your breath.",
                    "The world slows down... just for a bit.",
                    "Peace and quiet embrace you here.",
                    "No threats. No worries. Just rest.",
                    "You feel your strength slowly returning.",
                    "Time seems to pause as you relax.",
                    "Your heartbeat steadies, your mind clears.",
                    "A calm breeze whispers: 'You've earned this."
                }
            }
        }
    };

    public Dictionary<string, EffectData> items = new Dictionary<string, EffectData>()
    {
        {
            "stTrina",
            new EffectData
            {
                 mainText = "Used St.Trina flower",
                flavorTexts = new string[]
                {
                    "You shake the flower, its scent lulls Cellica to sleep.",
                }
            }
        },

        {
            "potOfGreed",
            new EffectData
            {
                 mainText = "Used Pot of Greed",
                flavorTexts = new string[]
                {
                    "Your greed grants you an extra turn or a bonus... \nYou'll see.",
                    "You summon Pot of Greed... an extra turn or a bonus awaits.",
                }
            }
        },

        {
            "mouthWash",
            new EffectData
            {
                 mainText = "Used Mouth Wash",
                flavorTexts = new string[]
                {
                    "Mouthwash does nothing. But hey, hope is powerful!",
                }
            }
        },

        {
            "allenM60",
            new EffectData
            {
                 mainText = "Used Allen M60",
                flavorTexts = new string[]
                {
                    "You inspect the rusted M60. It’s completely unusable.",
                }
            }
        }
    };

    public string GetEffect(string effectKey, int type)
    {
        Dictionary<string, EffectData> source = null;
        if (type == 0)
        {
            source = rest;
        }
        else if (type == 1)
        {
            source = bonuses;
        }
        else if (type == 2)
        {
            source = curses;
        }
        else if (type == 3)
        {
            source = items;
        }

        if (!source.ContainsKey(effectKey))
        {
            Debug.LogWarning($"Effect key not found: {effectKey}");
            return "";
        }

        EffectData data = source[effectKey];
        string flavor = data.flavorTexts[Random.Range(0, data.flavorTexts.Length)];

        return $"{data.mainText}\n\n{flavor}";
    }

    private void Awake()
    {
        Instance = this;
    }


    [ServerRpc(RequireOwnership = false)]
    public void GetMessageServerRpc(FixedString128Bytes effectKey, int type)
    {
        FixedString128Bytes effect = new FixedString128Bytes(GetEffect(effectKey.ToString(), type));
        SetUICLientRpc(effectKey, effect, type);
    }

    [ClientRpc]
    public void SetUICLientRpc(FixedString128Bytes effectKey, FixedString128Bytes effect, int type)
    {
        HideHelpRequestClientRpc();
        
        spawnedUI = Instantiate(MessageUIPrefab,canvasTransform);
        var ui = spawnedUI.GetComponent<CurseBonusUI>();

        ui.SetText(effect.ToString(), type);

        StartCoroutine(DestroyAfterDelay(spawnedUI, 6.2f, effectKey.ToString(), type));
    
        ShowHelpRequestClientRpc();
    }

    public void SetUI(FixedString128Bytes effectKey, int type)
    {
        HideHelpRequestClientRpc();

        FixedString128Bytes effect = new FixedString128Bytes(GetEffect(effectKey.ToString(), type));

        spawnedUI = Instantiate(MessageUIPrefab, canvasTransform);
        var ui = spawnedUI.GetComponent<CurseBonusUI>();

        ui.SetText(effect.ToString(), type);

        StartCoroutine(DestroyAfterDelay(spawnedUI, 6.2f, effectKey.ToString(), type));
        
        ShowHelpRequestClientRpc();
    }

    private IEnumerator DestroyAfterDelay(GameObject uiObject, float delay, string effectKey, int type)
    {
        yield return new WaitForSeconds(delay);
        if (uiObject != null)
        {
            Destroy(uiObject);
        }

        if (RolesManager.IsMyTurn && effectKey != "reposition" && effectKey != "potOfGreed" && effectKey != "mouthWash" && effectKey != "allenM60")
        {
            StartCoroutine(DelaySwitchTurn());
        }
    }

    public void StartRepositionCoroutine(float delay)
    {
        StartCoroutine(CurseTileEvent.DelayReposition(delay));
    }

    private IEnumerator DelaySwitchTurn()
    {
        yield return new WaitForSeconds(1f);
        RolesManager.SwitchRole();
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayBonusSFXServerRpc()
    {
        PlayBonusSFXClientRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayCurseSFXServerRpc()
    {
        PlayCurseSFXClientRpc();
    }

    [ClientRpc]
    private void PlayBonusSFXClientRpc()
    {
        AudioManager.instance?.PlaySFX(AudioManager.instance.itemEffectSFX);
    }

    [ClientRpc]
    private void PlayCurseSFXClientRpc()
    {
        AudioManager.instance?.PlaySFX(AudioManager.instance.damageTakenSFX);
    }

    [ClientRpc]
    private void HideHelpRequestClientRpc()
    {
        if (HelpRequest != null)
            HelpRequest.SetActive(false);
    }

    [ClientRpc]
    private void ShowHelpRequestClientRpc()
    {
        if (HelpRequest != null)
            HelpRequest.SetActive(true);
    }

}
