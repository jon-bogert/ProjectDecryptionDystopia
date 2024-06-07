using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace XephTools
{
    public interface ISequenceable
    {
        public void SequenceValue(float val);
        public void Complete(bool isOpen);
        public float BeginGetLength(bool isOpening);
    }

    public enum SequenceType { Open, Close };

    public class UIEventSequencer : MonoBehaviour
    {
        [System.Serializable]
        public struct Sequence
        {
            public string name;
            public SequenceType type;
            public List<GameObject> sequenceables;
            [Space]
            [Header("End Event")]
            public UnityEvent OnComplete;
        }
        public class SequenceHashed
        {
            public SequenceType type;
            public List<ISequenceable> sequenceables = new();
            public UnityEvent OnComplete = null;
        }

        [SerializeField] List<Sequence> _sequences = new();

        Dictionary<string, SequenceHashed> _sequencesDict = new();
        List<ISequenceable> _activeSequence = null;
        UnityEvent _activeCompleteEvent = null;
        SequenceType _activeType = SequenceType.Open;

        public bool hasActiveSequence { get { return _activeSequence != null; } }

        private void Awake()
        {
            foreach (Sequence s in _sequences)
            {
                SequenceHashed hashed = new();

                if (s.sequenceables.Count == 0)
                {
                    Debug.LogError(s.name + " sequence does not contain any elements");
                    continue;
                }

                hashed.OnComplete = s.OnComplete;
                hashed.type = s.type;
                foreach (GameObject go in s.sequenceables)
                {
                    ISequenceable sequenceable = go.GetComponent<ISequenceable>();
                    if (sequenceable == null)
                    {
                        Debug.LogError(go.name + " has no Component that inherits from ISequenceable");
                        continue;
                    }
                    hashed.sequenceables.Add(sequenceable);
                }
                _sequencesDict.Add(s.name, hashed);
            }
        }

        public void StartSequence(string name)
        {
            if (hasActiveSequence)
                return;

            SequenceHashed hashed = _sequencesDict[name];
            if (hashed == null)
            {
                Debug.LogError("Could not find sequence with name " + name);
                return;
            }

            _activeSequence = new List<ISequenceable>();
            _activeSequence.AddRange(hashed.sequenceables);
            _activeCompleteEvent = hashed.OnComplete;
            _activeType = hashed.type;

            StartNext();
        }

        private void StartNext()
        {
            bool isOpening = _activeType == SequenceType.Open;
            float start = (isOpening) ? 0f : 1f;
            float end = (isOpening) ? 1f : 0f;

            ISequenceable current = _activeSequence[0];

            OverTime.LerpModule lerp = new(start, end, current.BeginGetLength(isOpening), current.SequenceValue);
            lerp.OnComplete(Next);
            OverTime.Add(lerp);
        }

        private void Next()
        {
            _activeSequence[0].Complete(_activeType == SequenceType.Open);

            _activeSequence.RemoveAt(0);
            if (_activeSequence.Count > 0)
            {
                StartNext();
                return;
            }

            _activeSequence = null;
            _activeCompleteEvent?.Invoke();
            _activeCompleteEvent = null;
        }
    }
}
