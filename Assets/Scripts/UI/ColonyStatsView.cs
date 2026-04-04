using R3;
using Stats;
using TMPro;
using UnityEngine;
using Zenject;

namespace UI
{
    public class ColonyStatsView : MonoBehaviour
    {
        private const string DeadWorkersFormat = "Dead Workers: {0}";
        private const string DeadPredatorsFormat = "Dead Predators: {0}";

        [SerializeField] private TMP_Text _deadWorkersText;
        [SerializeField] private TMP_Text _deadPredatorsText;

        private IColonyStatsService _statsService;
        private CompositeDisposable _disposables = new();

        [Inject]
        private void Construct(IColonyStatsService statsService)
        {
            _statsService = statsService;
        }

        private void Start()
        {
            _statsService.DeadWorkers
                .Subscribe(count => _deadWorkersText.text = string.Format(DeadWorkersFormat, count))
                .AddTo(_disposables);

            _statsService.DeadPredators
                .Subscribe(count => _deadPredatorsText.text = string.Format(DeadPredatorsFormat, count))
                .AddTo(_disposables);
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
            _disposables = null;
        }
    }
}
