using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XephTools;

public class DoorKey : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] float _collectAnimationTime = 1.5f;
    [SerializeField] float _collectAnimationHeight = 1f;
    [SerializeField] float _collectSpinDegrees = 720f;
    [Header("Haptics")]
    [SerializeField] float _hapticIntensity = 0.2f;
    [SerializeField] float _hapticDuration = 0.05f;

    DoorLock _lock;
    SoundPlayer3D _soundPlayer;
    ValueMover _valueMover;

    KeyBob _keyBob;

    bool _isCollected = false;

    List<Material> _materials = new();
    InteractionPoint[] _interactionPoints;

    private void Awake()
    {
        _keyBob = GetComponent<KeyBob>();
        if (_keyBob == null)
        {
            Debug.LogError(name + ": could not find KeyBob component");
        }

        foreach (MeshRenderer renderer in GetComponentsInChildren<MeshRenderer>())
        {
            _materials.AddRange(renderer.materials.ToList());
        }
    }

    private void Start()
    {
        _lock = FindObjectOfType<DoorLock>();
        if (_lock == null )
        {
            Debug.LogError(name + " could not find Door in scene");
            return;
        }

        _lock.AddKey(this);

        _soundPlayer = FindObjectOfType<SoundPlayer3D>();
        if (_soundPlayer == null)
            Debug.LogError("Couldn't find Sound Player in Scene");

        _interactionPoints = FindObjectsOfType<InteractionPoint>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isCollected)
            return;

        ThirdPersonMovement player = other.GetComponent<ThirdPersonMovement>();
        if (player == null)
            return;

        _soundPlayer.Play("key-collect", transform.position, SoundPlayer3D.Bank.Single);
        _isCollected = true;
        Collect();
    }

    private void Collect()
    {
        _lock.RemoveKey(this);

        foreach (InteractionPoint ip in _interactionPoints)
        {
            ip.SendHaptic(_hapticIntensity, _hapticDuration);
        }
        
        float startY = transform.position.y;

        OverTime.LerpModule sineLerp = new(0f, 1f, _collectAnimationTime, (float t) =>
        {
            transform.position = 
            new (
                transform.position.x,
                startY + Mathf.Sin(t * Mathf.PI) * _collectAnimationHeight,
                transform.position.z
            );
        });

        OverTime.LerpModule materialLerp = new(1f, 0f, _collectAnimationTime, (float t) =>
        {
            foreach (Material mat in _materials)
            {
                mat.SetFloat("_Dissolve", t);
            }
        });

        float startRot = transform.rotation.eulerAngles.y;
        OverTime.CubicLerpModule rotLerp = new(
            startRot,
            startRot + _collectSpinDegrees,
            startRot + 10,
            startRot + _collectSpinDegrees - 10, 
            _collectAnimationTime,
            (float r) =>
            {
                transform.eulerAngles = new(
                    transform.eulerAngles.x,
                    r,
                    transform.eulerAngles.z
                );
            });
        rotLerp.OnComplete(() => { gameObject.SetActive(false); });

        OverTime.Add(sineLerp);
        OverTime.Add(materialLerp);
        OverTime.Add(rotLerp);
    }
}
