using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class spawns random pieces.
/// </summary>
public class BlocksManager : MonoBehaviour
{
    public static BlocksManager instance = null;

    [SerializeField] int pieceTimeToFall;
    [SerializeField] private List<GameObject> piecePrefabs;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => GridManager.instance.isActiveAndEnabled);

        SpawnPiece();
    }

    private void OnEnable()
    {
        PieceBehaviour.OnReachBottom += SpawnPiece;
    }

    private void OnDisable()
    {
        PieceBehaviour.OnReachBottom -= SpawnPiece;
    }

    public void SpawnPiece()
    {
        GridManager gridManager = GridManager.instance;

        GameObject chosenPrefab = piecePrefabs[Random.Range(0, 6)];

        Vector3 spawnPoint = new Vector3((int)gridManager.transform.position.x + (float)gridManager.SizeX / 2,
                                         (int)gridManager.transform.position.y + (float)gridManager.SizeY - 1,
                                         (int)gridManager.transform.position.z);

        GameObject piece = Instantiate(chosenPrefab, spawnPoint, Quaternion.identity);

        piece.GetComponent<PieceBehaviour>().Fall(pieceTimeToFall);
    }
}
