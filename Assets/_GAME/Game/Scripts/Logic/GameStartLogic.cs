using _GAME.Audio;
using _GAME.Enemy;
using _GAME.Level;
using _GAME.Player;
using _GAME.Preload;
using _GAME.RateUs;
using _GAME.SaveAndLoadData;
using _GAME.Shop;
using _GAME.VibroService;
using _GAME.Wallet;
using UnityEngine;

namespace _GAME
{
    public class GameStartLogic : MonoBehaviour
    {
        private GameFeature _gameFeature;
        private WeaponHubFeature _weaponHubFeature;
        private WeaponsFeature _weaponsFeature;
        private SaveAndLoadDataFeature _saveAndLoadDataFeature;
        private ShopFeature _shopFeature;
        private WalletFeature _walletFeature;
        private LevelFeature _levelFeature;
        private VibroFeature _vibroFeature;
        private AudioFeature _audioFeature;
        private PreloadFeature _preloadFeature;
        private RateUsFeature _rateUsFeature;

        private PlayerFeature _playerFeature;
        private EnemyFeature _enemyFeature;

        private LevelUIView.LevelUIFeature _levelUIFeature;

        private Tutorial.TutorialFeature _tutorialFeature;

        private void Awake()
        {
            _gameFeature = FindObjectOfType<GameFeature>();

            Input.multiTouchEnabled = false;

            _weaponHubFeature = FindObjectOfType<WeaponHubFeature>();
            _weaponsFeature = FindObjectOfType<WeaponsFeature>();

            _saveAndLoadDataFeature = FindObjectOfType<SaveAndLoadData.SaveAndLoadDataFeature>();
            _shopFeature = FindObjectOfType<ShopFeature>();
            _walletFeature = FindObjectOfType<WalletFeature>();
            _levelFeature = FindObjectOfType<LevelFeature>();
            _vibroFeature = FindObjectOfType<VibroFeature>();
            _audioFeature = FindObjectOfType<AudioFeature>();
            _preloadFeature = FindObjectOfType<PreloadFeature>();
            _rateUsFeature = FindObjectOfType<RateUsFeature>();

            _playerFeature = FindObjectOfType<Player.PlayerFeature>();
            _enemyFeature = FindObjectOfType<Enemy.EnemyFeature>();

            _levelUIFeature = FindObjectOfType<LevelUIView.LevelUIFeature>();

            _tutorialFeature = FindObjectOfType<Tutorial.TutorialFeature>();

            GameFeature.Instance = _gameFeature;

            GameFeature.SaveAndLoadDataFeature = _saveAndLoadDataFeature;
            GameFeature.ShopFeature = _shopFeature;
            GameFeature.WalletFeature = _walletFeature;
            GameFeature.LevelFeature = _levelFeature;
            GameFeature.VibroFeature = _vibroFeature;
            GameFeature.AudioFeature = _audioFeature;
            GameFeature.PreloadFeature = _preloadFeature;
            GameFeature.RateUsFeature = _rateUsFeature;

            GameFeature.PlayerFeature = _playerFeature;
            GameFeature.EnemyFeature = _enemyFeature;
            GameFeature.LevelUIFeature = _levelUIFeature;
            GameFeature.WeaponHubFeature = _weaponHubFeature;
            GameFeature.WeaponsFeature = _weaponsFeature;

            GameFeature.TutorialFeature = _tutorialFeature;
    }

        private void Start()
        {
            //if (_preloadFeature) return;

            _gameFeature.OnStartGame?.Invoke();
        }

        private void OnEnable()
        {
            //if (_preloadFeature)
                //_preloadFeature.OnGameSceneLoaded += () => OnStart();

            _gameFeature.OnStartGame += () => _saveAndLoadDataFeature.OnLoadData?.Invoke();

            //Audio subscribe
            _audioFeature.OnSaveMusicSettings += () => _saveAndLoadDataFeature.OnSaveData?.Invoke();

            //VibroService subscribe
            _vibroFeature.OnSave += () => _saveAndLoadDataFeature.OnSaveData?.Invoke();

            //LevelSpawner subscribe
            _saveAndLoadDataFeature.OnDataLoaded += () => _levelFeature.OnLoadData?.Invoke(_saveAndLoadDataFeature.Data.LevelData);

            _levelFeature.OnSaveLevelData += () => _saveAndLoadDataFeature.OnSaveData?.Invoke();

            _saveAndLoadDataFeature.OnPrepareDataForSaving += () =>
            {
                _saveAndLoadDataFeature.Data.LevelData = _levelFeature.OnGetLevelData?.Invoke();
            };

            //Wallet subscribe
            _saveAndLoadDataFeature.OnDataLoaded += () => _walletFeature.OnDataLoaded?.Invoke();
            _walletFeature.OnSaveData += () =>
            {
                _saveAndLoadDataFeature.Data.CoinsCount = _walletFeature.CoinsInWallet;
                _saveAndLoadDataFeature.OnSaveData?.Invoke();
            };

            _walletFeature.OnPlaySound += () => _audioFeature.PlaySound(Audio.EnumSound.Money);

            //Shop subscribe
            _saveAndLoadDataFeature.OnDataLoaded += () => _shopFeature.OnLoadShopData?.Invoke(_saveAndLoadDataFeature.Data.ShopData);
            _shopFeature.OnSave += () => _saveAndLoadDataFeature.OnSaveData?.Invoke();
            _saveAndLoadDataFeature.OnPrepareDataForSaving += () =>
            {
                _saveAndLoadDataFeature.Data.ShopData = _shopFeature.OnGetShopData?.Invoke();
            };

            _shopFeature.OnGetMoneyFromWallet += () => { return _walletFeature.CoinsInWallet; };
            _shopFeature.OnBuyFromWallet += (count) => _walletFeature.OnBuy?.Invoke(count);
            _levelFeature.OnLevelLoaded += (lvl) => _shopFeature.OnUpdateOpenShopButtons?.Invoke();
            _shopFeature.OnPlayOpenSkinSound += () => _audioFeature.PlaySound(Audio.EnumSound.OpenSkin, volume: 0.7f);

            //RateUs subscribe
            _saveAndLoadDataFeature.OnDataLoaded += () => _rateUsFeature.OnLoadData?.Invoke(_saveAndLoadDataFeature.Data.RateUsData);
            _saveAndLoadDataFeature.OnPrepareDataForSaving += () =>
            {
                _saveAndLoadDataFeature.Data.RateUsData = _rateUsFeature.OnSaveData?.Invoke();
            };
            _rateUsFeature.OnSave += () => _saveAndLoadDataFeature.OnSaveData?.Invoke();
        }
    }
}
