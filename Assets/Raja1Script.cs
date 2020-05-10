using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Raja1Script : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public HealthBarScript healthbar;

    public AudioSource hitEffect;
    void Start()
    {
        currentHealth = maxHealth;
        healthbar.SetMaxHealth(maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        //if(currentHealth = 0) 
        {
            //SceneManager.LoadScene("ResultScene");
        }
    }
    private void OnCollisionEnter2D(Collision2D other){
    	if(other.collider.name == "raja1")
        {
            TakeDamage(20);
        }if(other.collider.tag=="Player"){
            hitEffect.Play();
		} 
    }
    
    void TakeDamage(int damage)
    {
        currentHealth -= damage;

        healthbar.SetHealth(currentHealth);
    }
}
