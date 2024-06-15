using Pelumi.ObjectPool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakablePieces : MonoBehaviour,IHasPool
{
    [SerializeField]
    public class Piece
    {
        [SerializeField] private Collider collider;
        [SerializeField] private Vector3 position;
        [SerializeField] private Quaternion rotation;
        [SerializeField] private Rigidbody rigidbody;

        public Collider Collider => collider;
        public Vector3 Position => position;
        public Quaternion Rotation => rotation;
        public Rigidbody Rigidbody => rigidbody;

        public Piece(GameObject gameObject)
        {
            this.collider = gameObject.GetComponent<Collider>();
            position = gameObject.transform.localPosition;
            rotation = gameObject.transform.localRotation;
            rigidbody = gameObject.GetComponent<Rigidbody>();
        }
    }

    [SerializeField] private float cutForce = 10;
    [SerializeField] private float lifeTime = 5;
    [SerializeField] private List<GameObject> parts = new List<GameObject>();
    [SerializeField] private List<Piece> pieces = new List<Piece>();

    public List<Piece> GetPieces => pieces;
    public float GetLifeTime => lifeTime;

    private void Awake()
    {
        foreach (GameObject part in parts)
        {
            pieces.Add(new Piece(part));
        }
    }

    public void Cut()
    {
        foreach (Piece piece in pieces)
        {
            piece.Rigidbody.detectCollisions = true;
            piece.Rigidbody.useGravity = true;
            piece.Rigidbody.isKinematic = false;

            // apply a 360 degree force to the pieces
            float randomX = Random.Range(-1f, 1f);
            float randomY = Random.Range(-1f, 1f);
            float randomZ = Random.Range(-1f, 1f);
            piece.Rigidbody.AddForce(new Vector3(randomX, randomY, randomZ) * cutForce, ForceMode.Impulse);
        }

        StartCoroutine(LifeTime());
    }

    IEnumerator LifeTime ()
    {
        yield return new WaitForSeconds(lifeTime);

        float scaleDuration = 1.5f;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / scaleDuration;
            foreach (Piece piece in pieces)
            {
                piece.Rigidbody.gameObject.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);
            }
            yield return null;
        }

        ObjectPoolManager.ReleaseObject(gameObject);
    }

    public void ResetPieses()
    {
        foreach (Piece piece in pieces)
        {
            piece.Rigidbody.detectCollisions = false;
            piece.Rigidbody.useGravity = false;
            piece.Rigidbody.isKinematic = true;
            piece.Rigidbody.gameObject.transform.localPosition = piece.Position;
            piece.Rigidbody.gameObject.transform.localRotation = piece.Rotation;
            piece.Rigidbody.gameObject.transform.localScale = Vector3.one;
        }
    }

    public void OnGet()
    {
        ResetPieses();
    }

    public void OnRelease()
    {

    }
}
