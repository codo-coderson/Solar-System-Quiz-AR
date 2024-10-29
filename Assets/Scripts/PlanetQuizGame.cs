using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

public class PlanetQuizGame : MonoBehaviour
{
    public GameObject[] celestialObjects; // Assign your planets here
    public Text displayText;
    public Text countdownText;
    private int score = 0;
    private int currentRound = 0;
    private GameObject currentTarget;

    void Start()
    {
        StartCoroutine(QuizRounds());
    }

    IEnumerator QuizRounds()
    {
        while (currentRound < 5)
        {
            // Choose a random celestial object
            currentTarget = celestialObjects[Random.Range(0, celestialObjects.Length)];
            displayText.text = $"Enfoca el cuerpo celeste: {currentTarget.name}";
            yield return new WaitForSeconds(2);

            // Countdown starts
            StartCoroutine(Countdown(20));

            // Wait for the student to focus the correct image or until countdown ends
            bool correctFocus = false;
            float elapsedTime = 0;
            while (elapsedTime < 20 && !correctFocus)
            {
                elapsedTime += Time.deltaTime;
                foreach (GameObject obj in celestialObjects)
                {
                    var observer = obj.GetComponent<DefaultObserverEventHandler>();
                    if (observer && observer.HasBeenFound() && obj == currentTarget)
                    {
                        correctFocus = true;
                        break;
                    }
                }
                yield return null;
            }

            if (correctFocus)
            {
                score++;
                displayText.text = $"Correcto! +1 punto. Puntaje: {score}";
            }
            else
            {
                score--;
                displayText.text = $"Incorrecto o tiempo acabado! -1 punto. Puntaje: {score}";
            }
            currentRound++;
            yield return new WaitForSeconds(2);
        }

        displayText.text = $"Juego terminado! Puntaje final: {score}";
    }

    IEnumerator Countdown(int time)
    {
        while (time > 0)
        {
            countdownText.text = $"Tiempo restante: {time}";
            yield return new WaitForSeconds(1);
            time--;
        }
        countdownText.text = "Tiempo terminado!";
    }
}

public class DefaultObserverEventHandler : MonoBehaviour
{
    private ObserverBehaviour mObserverBehaviour;
    private bool hasBeenFound = false;

    void Start()
    {
        mObserverBehaviour = GetComponent<ObserverBehaviour>();
        mObserverBehaviour.OnTargetStatusChanged += OnTargetStatusChanged;
    }

    private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus targetStatus)
    {
        if (targetStatus.Status == Status.TRACKED || targetStatus.Status == Status.EXTENDED_TRACKED)
        {
            hasBeenFound = true;
        }
        else
        {
            hasBeenFound = false;
        }
    }

    public bool HasBeenFound()
    {
        return hasBeenFound;
    }
}
