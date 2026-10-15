using UnityEngine;
using Zenject;

public class TurretLaser : MonoBehaviour
{
    [SerializeField] private Shader _shader;
    [SerializeField] private Color _color = new Color(1f, 0.1f, 0.1f, 0.7f);
    [SerializeField, Min(0f)] private float _length = 30f;
    [SerializeField, Min(0.001f)] private float _width = 0.025f;

    private GameController _game;
    private LineRenderer _line;
    private Material _material;

    [Inject]
    public void Construct(GameController game)
    {
        _game = game;
    }

    private void Awake()
    {
        _material = new Material(_shader);
        _line = gameObject.AddComponent<LineRenderer>();
        _line.sharedMaterial = _material;
        _line.useWorldSpace = true;
        _line.positionCount = 2;
        _line.startWidth = _width;
        _line.endWidth = _width;
        _line.startColor = _color;
        _line.endColor = _color;
        _line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        _line.receiveShadows = false;
        _line.enabled = false;
    }

    private void LateUpdate()
    {
        _line.enabled = _game.IsPlaying;
        if (!_line.enabled)
            return;

        Vector3 endPoint = transform.position + transform.forward * _length;
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit,
            _length, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide))
        {
            endPoint = hit.point;
        }

        _line.SetPosition(0, transform.position);
        _line.SetPosition(1, endPoint);
    }

    private void OnDestroy()
    {
        if (_material != null)
            Destroy(_material);
    }
}
