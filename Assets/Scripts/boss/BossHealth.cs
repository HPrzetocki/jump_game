using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [Header("Statystyki")]
    public int health = 500;

    [Header("Śmierć")]
    public GameObject deathEffect;

    [Header("Nietykalność / obrażenia")]
    public bool isInvulnerable = false;

    [Header("Miganie koloru po trafieniu")]
    public SpriteRenderer bossSprite;      // przypisz SpriteRenderer bossa
    public Color hitColor = new Color(1f, 0.3f, 0.3f, 1f); // lekko czerwony
    public float hitFlashTime = 0.1f;      // czas "czerwonego"

    private Color _originalColor;
    private Coroutine _hitFlashCoroutine;

    void Start()
    {
        if (bossSprite == null)
        {
            bossSprite = GetComponent<SpriteRenderer>();
        }

        if (bossSprite != null)
        {
            _originalColor = bossSprite.color;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvulnerable)
            return;

        health -= damage;

        // miganie po otrzymaniu obrażeń
        if (bossSprite != null)
        {
            // jeśli poprzednie miganie jeszcze trwa – przerwij, żeby nie nadpisywały się
            if (_hitFlashCoroutine != null)
            {
                StopCoroutine(_hitFlashCoroutine);
            }
            _hitFlashCoroutine = StartCoroutine(HitFlash());
        }

        if (health <= 200)
        {
            GetComponent<Animator>().SetBool("IsEnraged", true);
        }

        if (health <= 0)
        {
            Die();
        }
    }

    IEnumerator HitFlash()
    {
        // ustaw kolor na czerwony
        bossSprite.color = hitColor;

        // czekaj chwilę
        yield return new WaitForSeconds(hitFlashTime);

        // przywróć oryginalny kolor
        bossSprite.color = _originalColor;

        _hitFlashCoroutine = null;
    }

    void Die()
    {
        Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
