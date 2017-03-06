using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TextureGenerator : MonoBehaviour {

    [SerializeField] string filename;

	IEnumerator Start () {
        yield return new WaitForSeconds(0.3f);
        GenerateTexture();
	}

    public void GenerateTexture(){
        Texture2D displayTexture = new Texture2D(400, 1, TextureFormat.ARGB32, false);

        for (int i = 0; i < displayTexture.width; i++){
            displayTexture.SetPixel(i, 0, Color.HSVToRGB(i / (displayTexture.width - 1f), 1f, 1f));
        }
        displayTexture.Apply();

        File.WriteAllBytes(Application.dataPath + "/" + filename + ".png", displayTexture.EncodeToPNG());
    }
}
