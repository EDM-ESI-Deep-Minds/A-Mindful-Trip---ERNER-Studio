using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class RoomUIManager : NetworkBehaviour
{
    [SerializeField] private Transform[] playerSlots;
    [SerializeField] private Sprite[] characterSprites;

    private List<string> playerNames = new List<string>(new string[4]);
    private List<int> playerCharacters = new List<int>(new int[4]);

    private bool[] characterTaken = new bool[4];
    private int[] selectedCharacters = new int[4];

    private void Awake()
    {
        for (int i = 0; i < 4; i++)
        {
            playerNames[i] = "";
            playerCharacters[i] = -1;
            selectedCharacters[i] = -1;
            UpdatePlayerUI(i);
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            if (ProfileManager.SelectedProfile == null)
            {
                SendProfileToServerServerRpc(PlayerPrefs.GetString("PlayerName"), PlayerPrefs.GetInt("Character"));
            }
            else
            {
                SendProfileToServerServerRpc(ProfileManager.SelectedProfile.playerName, ProfileManager.SelectedProfile.character);
            }
        }

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += HandlePlayerDisconnect;
        }
    }

    public override void OnDestroy()
    {
        if (IsServer && NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandlePlayerDisconnect;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendProfileToServerServerRpc(string playerName, int character, ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        int emptySlot = GetFirstEmptySlot();
        if (emptySlot == -1)
        {
            Debug.LogError("No available slots for the new player!");
            return;
        }

        int assignedCharacter = GetNextAvailableCharacter(character);
        if (assignedCharacter == -1)
        {
            Debug.LogError("No available characters!");
            return;
        }

        playerNames[emptySlot] = playerName;
        playerCharacters[emptySlot] = assignedCharacter;
        characterTaken[assignedCharacter] = true;
        selectedCharacters[emptySlot] = assignedCharacter;

        Debug.Log($"Assigned Player {playerName} to slot {emptySlot}");

        UpdateAllClientsClientRpc(ToSerializedString(playerNames), playerCharacters.ToArray(), characterTaken);
    }

    [ClientRpc]
    private void UpdateAllClientsClientRpc(string serializedNames, int[] characters, bool[] takenCharacters)
    {
        playerNames = FromSerializedString(serializedNames);
        playerCharacters = characters.ToList();
        characterTaken = takenCharacters;
        selectedCharacters = characters;

        for (int i = 0; i < 4; i++)
        {
            UpdatePlayerUI(i);
        }
    }

    private int GetFirstEmptySlot()
    {
        for (int i = 0; i < 4; i++)
        {
            if (string.IsNullOrEmpty(playerNames[i]))
                return i;
        }
        return -1;
    }

    private void UpdatePlayerUI(int index)
    {
        if (playerSlots == null || index < 0 || index >= playerSlots.Length) return;

        Transform slot = playerSlots[index];
        if (slot == null) return;

        Transform playerBanner = slot.Find("PlayerBanner");
        if (playerBanner == null) return;

        TextMeshProUGUI nameText = playerBanner.Find("PlayerName")?.GetComponent<TextMeshProUGUI>();
        Image characterImage = slot.Find("CharacterImage")?.GetComponent<Image>();
        Button changeButton = slot.Find("ChangeCharacterButton")?.GetComponent<Button>();

        if (nameText != null)
        {
            nameText.text = string.IsNullOrEmpty(playerNames[index]) ? "" : playerNames[index];
        }

        if (characterImage != null)
        {
            int characterIndex = playerCharacters[index];
            if (characterIndex >= 0 && characterIndex < characterSprites.Length)
            {
                characterImage.sprite = characterSprites[characterIndex];
                characterImage.color = string.IsNullOrEmpty(playerNames[index]) ? Color.black : Color.white;
            }
        }

        if (changeButton != null)
        {
            bool isLocalPlayer = playerNames[index] == ProfileManager.SelectedProfile?.playerName;
            changeButton.gameObject.SetActive(isLocalPlayer);
            changeButton.onClick.RemoveAllListeners();

            if (isLocalPlayer)
                changeButton.onClick.AddListener(() => ChangeCharacter(index));
        }
    }

    public void ChangeCharacter(int slotIndex)
    {
        if (!IsClient) return;

        int currentCharacter = playerCharacters[slotIndex];
        int newCharacter = GetNextAvailableCharacter((currentCharacter + 1) % characterSprites.Length);

        if (newCharacter == -1 || newCharacter == currentCharacter)
        {
            Debug.Log("No other characters available to switch to.");
            return;
        }

        ChangeCharacterServerRpc(slotIndex, newCharacter);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangeCharacterServerRpc(int slotIndex, int newCharacter)
    {
        if (slotIndex < 0 || slotIndex >= playerNames.Count || string.IsNullOrEmpty(playerNames[slotIndex]))
            return;

        int oldCharacter = playerCharacters[slotIndex];
        if (oldCharacter >= 0) characterTaken[oldCharacter] = false;

        playerCharacters[slotIndex] = newCharacter;
        characterTaken[newCharacter] = true;
        selectedCharacters[slotIndex] = newCharacter;

        UpdateAllClientsClientRpc(ToSerializedString(playerNames), playerCharacters.ToArray(), characterTaken);
    }

    private int GetNextAvailableCharacter(int startIndex)
    {
        for (int i = 0; i < characterSprites.Length; i++)
        {
            int index = (startIndex + i) % characterSprites.Length;
            if (!characterTaken[index])
                return index;
        }
        return -1;
    }

    private void HandlePlayerDisconnect(ulong clientId)
    {
        int playerIndex = playerNames.FindIndex(name => NetworkManager.Singleton.ConnectedClients.ContainsKey(clientId) &&
                                                        NetworkManager.Singleton.ConnectedClients[clientId].ClientId == clientId);
        if (playerIndex != -1)
        {
            int oldCharacter = playerCharacters[playerIndex];
            if (oldCharacter >= 0) characterTaken[oldCharacter] = false;

            playerNames[playerIndex] = "";
            playerCharacters[playerIndex] = -1;
            selectedCharacters[playerIndex] = -1;

            UpdateAllClientsClientRpc(ToSerializedString(playerNames), playerCharacters.ToArray(), characterTaken);
        }
    }

    private string ToSerializedString(List<string> list)
    {
        return string.Join("|", list.Select(s => string.IsNullOrEmpty(s) ? "_" : s));
    }

    private List<string> FromSerializedString(string data)
    {
        return data.Split('|').Select(s => s == "_" ? "" : s).ToList();
    }

    public int[] GetSelectedCharacters()
    {
        return selectedCharacters;
    }
}