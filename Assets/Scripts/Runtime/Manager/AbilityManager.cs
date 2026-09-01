using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    #region 인스펙터
    [Header("플레이어")]
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Health _health;
    #endregion

    #region 내부 변수
    private float _damageMultiplier = 1f;
    private float _maxHealthMultiplier = 1f;
    private float _moveSpeedMultiplier = 1f;
    private float _fireRateMultiplier = 1f;
    private int _magazineBonus;
    #endregion

    private void Awake()
    {
        if (_playerController == null)
        {
            _playerController = GetComponent<PlayerController>();
        }

        if (_health == null)
        {
            _health = GetComponent<Health>();
        }
    }

    public void ApplyAbility(Ability ability, int diceResult)
    {
        if (ability == null)
        {
            return;
        }

        float upgradeRate = diceResult * 0.1f;

        switch (ability.Type)
        {
            case AbilityType.Damage:
                ApplyDamage(upgradeRate);
                break;

            case AbilityType.MaxHealth:
                ApplyMaxHealth(upgradeRate);
                break;
                
            case AbilityType.MoveSpeed:
                ApplyMoveSpeed(upgradeRate);
                break;

            case AbilityType.FireRate:
                ApplyFireRate(upgradeRate);
                break;

            case AbilityType.MagazineSize:
                ApplyMagazineSize(diceResult);
                break;
        }

        if (ability.Type == AbilityType.MagazineSize)
        {
            CPrint.Log($"어빌리티 적용 : {ability.AbilityName} / 증가량 : +{diceResult}");
        }
        else
        {
            CPrint.Log($"어빌리티 적용 : {ability.AbilityName} / 증가량 : +{upgradeRate * 100f}%");
        }
    }

    private void ApplyDamage(float amount)
    {
        _damageMultiplier += amount;

        foreach (Weapon weapon in _playerController.Weapons)
        {
            weapon.SetDamageMultiplier(_damageMultiplier);
        }

        CPrint.Log($"공격력 증가 : +{amount * 100f}%");
    }

    private void ApplyMaxHealth(float amount)
    {
        _maxHealthMultiplier += amount;

        _health.SetHealthMultiplier(_maxHealthMultiplier);

        CPrint.Log($"최대 체력 증가 : +{amount * 100f}%");
    }

    private void ApplyMoveSpeed(float amount)
    {
        _moveSpeedMultiplier += amount;

        _playerController.SetMoveSpeedMultiplier(_moveSpeedMultiplier);

        CPrint.Log($"이동 속도 증가 : +{amount * 100f}%");
    }

    private void ApplyFireRate(float amount)
    {
        _fireRateMultiplier += amount;

        foreach (Weapon weapon in _playerController.Weapons)
        {
            weapon.SetFireRateMultiplier(_fireRateMultiplier);
        }

        CPrint.Log($"연사속도 증가 : +{amount * 100f}%");
    }

    private void ApplyMagazineSize(int amount)
    {
        _magazineBonus += amount;

        foreach (Weapon weapon in _playerController.Weapons)
        {
            weapon.SetMagazineBonus(_magazineBonus);
        }

        CPrint.Log($"탄창 증가 : +{amount}");
    }

}
