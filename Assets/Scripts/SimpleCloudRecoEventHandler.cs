using UnityEngine;
using Vuforia;
using System.Collections;
using System.IO;
using UnityEngine.Networking;


public class PlanetInfo
{
    public string link;
    public string name;
    public string text;

    // TODO cambiale el nombre a PlayerInfo
    public static PlanetInfo CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<PlanetInfo>(jsonString);
    }

}


// Mecánica del juego 
// La lista de planetas a adivinar se especifica en un JSON que se carga al inicio de la aplicación y se parseará como un array de texto
// En la esquina superior izquierda habrá en todo momento un contador de planetas adivinados
// El texto "Encuentra el siguiente planeta: " + un planeta generado aleatoriamente se muestra en la parte superior de la pantalla
// Cada planeta adivinado sumará un punto al contador de planetas adivinados
// Si se falla, se mostrará un mensaje de "Error: el planeta era [nombre_del_planeta]", se mostrará la puntuación de esa partida y un botón táctil para empezar de nuevo
public class SimpleCloudRecoEventHandler : MonoBehaviour
{
    CloudRecoBehaviour mCloudRecoBehaviour;
    bool mIsScanning = false;
    public static string mTargetMetadata = "";
    PlanetInfo planet;



    public ImageTargetBehaviour ImageTargetTemplate;

    // Register cloud reco callbacks
    void Awake()
    {
        mCloudRecoBehaviour = GetComponent<CloudRecoBehaviour>();
        mCloudRecoBehaviour.RegisterOnInitializedEventHandler(OnInitialized);
        mCloudRecoBehaviour.RegisterOnInitErrorEventHandler(OnInitError);
        mCloudRecoBehaviour.RegisterOnUpdateErrorEventHandler(OnUpdateError);
        mCloudRecoBehaviour.RegisterOnStateChangedEventHandler(OnStateChanged);
        mCloudRecoBehaviour.RegisterOnNewSearchResultEventHandler(OnNewSearchResult);
    }
    //Unregister cloud reco callbacks when the handler is destroyed
    void OnDestroy()
    {
        mCloudRecoBehaviour.UnregisterOnInitializedEventHandler(OnInitialized);
        mCloudRecoBehaviour.UnregisterOnInitErrorEventHandler(OnInitError);
        mCloudRecoBehaviour.UnregisterOnUpdateErrorEventHandler(OnUpdateError);
        mCloudRecoBehaviour.UnregisterOnStateChangedEventHandler(OnStateChanged);
        mCloudRecoBehaviour.UnregisterOnNewSearchResultEventHandler(OnNewSearchResult);
    }
    public void OnInitialized(CloudRecoBehaviour cloudRecoBehaviour)
    {
        Debug.Log("Cloud Reco initialized");
    }

    public void OnInitError(CloudRecoBehaviour.InitError initError)
    {
        Debug.Log("Cloud Reco init error " + initError.ToString());
    }

    public void OnUpdateError(CloudRecoBehaviour.QueryError updateError)
    {
        Debug.Log("Cloud Reco update error " + updateError.ToString());

    }
    public void OnStateChanged(bool scanning)
    {
        mIsScanning = scanning;

        if (scanning)
        {
            // Clear all known targets
        }
    }
    // Here we handle a cloud target recognition event
    public void OnNewSearchResult(CloudRecoBehaviour.CloudRecoSearchResult cloudRecoSearchResult)
    {
        // Store the target metadata
        mTargetMetadata = cloudRecoSearchResult.MetaData;
        planet = PlanetInfo.CreateFromJSON(mTargetMetadata);

        // Stop the scanning by disabling the behaviour
        mCloudRecoBehaviour.enabled = false;
        // Build augmentation based on target 
        if (ImageTargetTemplate)
        {
            /* Enable the new result with the same ImageTargetBehaviour: */
            mCloudRecoBehaviour.EnableObservers(cloudRecoSearchResult, ImageTargetTemplate.gameObject);

            StartCoroutine(GetAssetBundle());
        }
    }

    IEnumerator GetAssetBundle()
    {
        // UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle("https://drive.google.com/uc?export=download&id=1JD6jhAPetCJOchgxQwYshuWXWE-V6B2a");
        // No queremos coger el asset bundle de un link de google drive, queremos cogerlo de los metadatos de la imagetarget en el servidor de vuforia -->
        UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(planet.link);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
            string[] allAssetNames = bundle.GetAllAssetNames();
                
            for(int i = 0; i < allAssetNames.Length; i++)
            {
                print(allAssetNames[i]);
            }
            // Se obtiene el nombre del objeto que se ha encontrado en el AssetBundle
            string gameObjectName = Path.GetFileNameWithoutExtension(allAssetNames[0]).ToString();
            // Se carga el objeto que se ha encontrado en el AssetBundle
            GameObject objectFound = bundle.LoadAsset(gameObjectName) as GameObject;
            // Se instancia el objeto que se ha encontrado en la posición y rotación del ImageTargetTemplate
            GameObject obj = Instantiate(objectFound, ImageTargetTemplate.transform.position, ImageTargetTemplate.transform.rotation);
            // Se establece como padre del objeto encontrado el ImageTargetTemplate
            obj.transform.parent = ImageTargetTemplate.transform;
        }
    }

    void OnGUI()
    {
        // Display current 'scanning' status
        GUI.Box(new Rect(100, 100, 200, 50), mIsScanning ? "Scanning" : "Not scanning");
        // Display metadata of latest detected cloud-target
        GUI.Box(new Rect(100, 200, 200, 50), "Metadata: " + mTargetMetadata);
        // If not scanning, show button
        // so that user can restart cloud scanning
        if (!mIsScanning)
        {
            if (GUI.Button(new Rect(100, 300, 200, 50), "Restart Scanning"))
            {
                // Reset Behaviour
                mCloudRecoBehaviour.enabled = true;
                mTargetMetadata = "";
            }
        }
    }
}