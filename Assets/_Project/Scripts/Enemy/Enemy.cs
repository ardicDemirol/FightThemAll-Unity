using System.Collections;
using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Animator[] animators;
    [SerializeField] private TMP_Text enemySizeText;
    [HideInInspector] public int EnemyCount;

    private static readonly WaitForSeconds _waitForOneSeconds = new(1f);

    private static readonly int AnimIDDead = Animator.StringToHash("isDead");
    private static readonly int AnimIDShoot = Animator.StringToHash("isShooting");

    private void Awake()
    {
        animators = GetComponentsInChildren<Animator>();
        EnemyCount = animators.Length;
        ChangeEnemyNumber(EnemyCount);
    }

    private void OnEnable() => SubscribeEvents();
    private void OnDisable() => UnSubscribeEvents();
    private void SubscribeEvents()
    {
        Signals.Instance.OnTriggerEnter += WarController;
        Signals.Instance.OnGetPlayerNumber += SetData;
        Signals.Instance.OnChangeEnemyNumber += ChangeEnemyNumber;
    }

    private void UnSubscribeEvents()
    {
        Signals.Instance.OnTriggerEnter -= WarController;
        Signals.Instance.OnGetPlayerNumber -= SetData;
        Signals.Instance.OnChangeEnemyNumber -= ChangeEnemyNumber;
    }

    private void ChangeEnemyNumber(int enemyCount)
    {
        //StringBuilder stringBuilder = new();
        //stringBuilder.Append(EnemyCount);
        //enemySizeText.text = stringBuilder.ToString();
        enemyCount = Mathf.Clamp(enemyCount, 0, EnemyCount);
        enemySizeText.text = enemyCount.ToString();
    }

    private void WarController()
    {
        foreach (var animator in animators)
        {
            animator.SetBool(AnimIDShoot, true);
        }
    }

    private void SetData(int arg0) => StartCoroutine(TakeDamage(arg0));

    public IEnumerator TakeDamage(int playerNumber)
    {
        yield return _waitForOneSeconds;

        if (playerNumber - EnemyCount > 0)
        {
            for (int i = 0; i < EnemyCount; i++)
            {
                Handheld.Vibrate();
                animators[i].SetBool(AnimIDDead, true);
                animators[i].GetComponentInChildren<Renderer>().material.SetColor("_Color", Color.gray);
            }
            Signals.Instance.OnPlayerWin?.Invoke();
        }
        else
        {
            for (int i = 0; i < playerNumber; i++)
            {
                Handheld.Vibrate();
                animators[i].SetBool(AnimIDDead, true);
                animators[i].GetComponentInChildren<Renderer>().material.SetColor("_Color", Color.gray);
            }
            Signals.Instance.OnPlayerLose?.Invoke();
        }

        ChangeEnemyNumber(EnemyCount - playerNumber);
    }
}
