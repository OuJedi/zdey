using UnityEngine;
using UnityEngine.Tilemaps;

public class CompositeColliderSetup : MonoBehaviour
{
  

    void Start()
    {
        // Ajouter Rigidbody2D et CompositeCollider2D au parent
        Rigidbody2D rb2d = gameObject.AddComponent<Rigidbody2D>();
        rb2d.bodyType = RigidbodyType2D.Static;
        CompositeCollider2D compositeCollider = gameObject.AddComponent<CompositeCollider2D>();

        // Trouver tous les TilemapCollider2D enfants et activer Used by Composite
        BoxCollider2D[] tilemapColliders = gameObject.GetComponentsInChildren<BoxCollider2D>();
        foreach (BoxCollider2D tilemapCollider in tilemapColliders)
        {
            tilemapCollider.compositeOperation = Collider2D.CompositeOperation.Merge;
        }
    }
}
