using UnityEngine;

namespace Prototype
{
    public class CardDatabase : MonoBehaviour
    {
        [Header("Data")] 
        [SerializeField] private TextAsset cardJson;
    }
}