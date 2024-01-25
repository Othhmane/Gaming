using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour {
    [SerializeField] private PlayerController _playerPrefab;

    public override void OnNetworkSpawn() {
        SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
    }



    // Assuming this is a class field to keep track of the number of players instantiated.
    private int playerCount = 0;
    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayerServerRpc(ulong playerId)
    {
        // Instantiate a player prefab. This assumes that _playerPrefab is assigned in the Unity Editor.
        var spawn = Instantiate(_playerPrefab);

        // Assuming that spawn has a NetworkObject component, spawn the object and assign ownership to the specified playerId.
        spawn.NetworkObject.SpawnWithOwnership(playerId);

        // Calculate the offset based on the number of players already instantiated.
        Vector3 offset = new Vector3(playerCount * 1f, 0f, 0f);

        // Modify the position of the spawned player by adding the calculated offset.
        spawn.transform.position += offset;

        // Increment the player count for the next instantiation.
        playerCount++;
    }


    public override void OnDestroy() {
        base.OnDestroy();
        MatchmakingService.LeaveLobby();
        if(NetworkManager.Singleton != null )NetworkManager.Singleton.Shutdown();
    }
}