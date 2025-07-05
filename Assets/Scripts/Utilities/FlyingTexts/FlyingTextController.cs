using TMPro;
using UnityEngine;
using UnityEngine.Pool;

namespace Utilities.FlyingTexts
{
    public class FlyingTextController : MonoBehaviour
    {
        [SerializeField] private FlyingText prefab;

        private ObjectPool<FlyingText> pool;
        private Camera camera;

        private void Awake()
        {
            camera = Camera.main;
            pool = new(CreateFlyingText, OnGetFlyingText, OnReleaseFlyingText, OnDestroyFlyingText);
        }

        private void OnDestroyFlyingText(FlyingText obj) => Destroy(obj.gameObject);

        private void OnReleaseFlyingText(FlyingText obj) => obj.gameObject.SetActive(false);

        private void OnGetFlyingText(FlyingText obj) => obj.gameObject.SetActive(true);

        private FlyingText CreateFlyingText() => Instantiate(prefab, transform, false);

        public TextMeshProUGUI Create(string text, Vector3 worldPosition, float duration = 1f)
        {
            var flyingText = pool.Get();

            //var screenPos = camera.WorldToScreenPoint(worldPosition);
            
            flyingText.Display(text, duration, worldPosition, pool.Release);
            return flyingText.Text;
        }
    }
}