using UnityEngine;
using System.Collections.Generic;
using Mirror;

// Classe permettant de creer des items
[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item", order = 1)]
public class Item : ScriptableObject
{ 
    public int id;
    public GameObject GFX;
    public string nom;
    public string description;
    public int rarete;
    public bool isUnique;
    public bool isPicked;
    public int cost;
    public float AP; //attaque physique
    public float AE; //attaque elementaire
    public float DEF; //defense physique
    public float DEF_ELE; //defense elementaire
    public float SPEED; //vitesse
    public float CDR; //cooldownreduction
    public float VAMP; //vampirisme
    public float HP; //health point
}
