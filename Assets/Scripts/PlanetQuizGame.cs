using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Vuforia;

public class PlanetQuizGame : MonoBehaviour
{
    
    [SerializeField] TextMeshProUGUI displayText;

    void Start()
    {
        GeneraYPideUnPlaneta();
    }

    public void GeneraYPideUnPlaneta()
    {
        string[] planets = {"Sol", "Mercurio", "Venus", "Tierra", "Marte", "Jupiter", "Saturno", "Urano", "Neptuno"};
        // wait for 2 seconds before showing planet name
        StartCoroutine(ShowPlanetName(planets));
     
    }

    public void detectaImagen(string targetID)
    {
        if (targetID == displayText.text)
        {
            displayText.text = "Correcto";
        }
        else
        {
            displayText.text = "Incorrecto";
        }
        StartCoroutine(RestartScene());
     
    }

    IEnumerator ShowPlanetName(string[] planets)
    {
        yield return new WaitForSeconds(2);
        displayText.text = planets[Random.Range(0, planets.Length)];
    }

    IEnumerator RestartScene()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

