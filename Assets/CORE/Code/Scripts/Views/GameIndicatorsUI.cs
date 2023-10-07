using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameIndicatorsUI : MonoBehaviour
{
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite halfHeart;
    [SerializeField] private Sprite emptyHeart;
    [SerializeField] private Sprite energyBand;
    [SerializeField] private Sprite fullEnergyBand;
    [Header("UI Elements")]
    [SerializeField] private TMP_Text textDiamondCounter;
    [SerializeField] private Image[] imageHearts;
    [SerializeField] private Image imageEnergyIndicator;
    [SerializeField] private TMP_Text textDaggerCounter;

    private bool lastEnergyIsMax;

    private void Awake()
    {
        Messenger<int>.AddListener(GameEvents.PLAYER_DAGGER_CHANGED, UpdateDaggerCounter);
        Messenger<int>.AddListener(GameEvents.DIAMOND_CHANGED, UpdateDiamondCounter);
        Messenger<int, int>.AddListener(GameEvents.PLAYER_HEALTH_CHANGED, UpdateHealthIndicator);
        Messenger<float, float>.AddListener(GameEvents.PLAYER_ENERGY_CHANGED, UpdateEnergyIndicator);

        Messenger.Broadcast(GameEvents.GAME_INDICATORS_STARTED);
    }

    private void OnDestroy()
    {
        Messenger<int>.RemoveListener(GameEvents.PLAYER_DAGGER_CHANGED, UpdateDaggerCounter);
        Messenger<int>.RemoveListener(GameEvents.DIAMOND_CHANGED, UpdateDiamondCounter);
        Messenger<int, int>.RemoveListener(GameEvents.PLAYER_HEALTH_CHANGED, UpdateHealthIndicator);
        Messenger<float, float>.RemoveListener(GameEvents.PLAYER_ENERGY_CHANGED, UpdateEnergyIndicator);
    }

    private void UpdateHealthIndicator(int hp, int maxHp)
    {
        int sectorSize = maxHp / imageHearts.Length;

        imageHearts.ToList().ForEach(e => e.sprite = emptyHeart);

        for (int i = 0; hp > 0; i++, hp -= sectorSize)
        {
            imageHearts[i].sprite = hp >= sectorSize ? fullHeart : halfHeart;
        }
    }

    private void UpdateEnergyIndicator(float energy, float maxEnergy)
    {
        if (energy != maxEnergy && lastEnergyIsMax)
        {
            imageEnergyIndicator.sprite = energyBand;
            lastEnergyIsMax = false;
        }
        if (energy == maxEnergy)
        {
            imageEnergyIndicator.sprite = fullEnergyBand;
            lastEnergyIsMax = true;
        }

        imageEnergyIndicator.fillAmount = energy / maxEnergy;
    }

    private void UpdateDaggerCounter(int daggerCount)
    {
        textDaggerCounter.text = daggerCount.ToString("D2");
    }

    private void UpdateDiamondCounter(int diamondCount)
    {
        textDiamondCounter.text = diamondCount.ToString("D5");
    }
}
