using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class SpriteCreator : MonoBehaviour
{
	//[SerializeField] private Texture2D imgBody;
	//[SerializeField] private Texture2D imgCostume;

	//[SerializeField] private AnimationClip animationClip;

	//[SerializeField] private SpriteRenderer spriteRenderer;
	
	//[SerializeField] private Animation animation;

	//public AnimationClip animClip;



	//private List<Sprite[]> sprites = new List<Sprite[]>();

	//void Start()
	//{
	//	int width = imgBody.width;
	//	int height = imgBody.height;

	//	Debug.Log("Start (test)");
	//	// Creation d'une image
	//	Texture2D img = new Texture2D(width, height);

	//	Color[] pixelImgBody = imgBody.GetPixels();
	//	img.SetPixels(pixelImgBody);
	//	img.Apply();

	//	// Ajout du costume
	//	Color[] pixelImgCostume = imgCostume.GetPixels();
	//	Debug.Log("Color 1 : " + pixelImgCostume[0]);
	//	for (int y = 0; y < height; y++)
	//	{
	//		for (int x = 0; x < width; x++)
	//		{
	//			if (pixelImgCostume[x + (y * width)].a > 0)
	//			{
	//				img.SetPixel(x, y, pixelImgCostume[x + (y * width)]);
	//			}
	//		}
	//	}

	//	img.Apply();

	//	// Création des 12 images à partir de l'image img
	//	Texture2D[] images = new Texture2D[12];
	//	for (int liney = 0; liney < 4; liney++)
	//	{
	//		for (int linex = 0; linex < 3; linex++)
	//		{
	//			Texture2D tempimg = new Texture2D(32, 32);
	//			for (int y = 0; y < 32; y++)
	//			{
	//				for (int x = 0; x < 32; x++)
	//				{
	//					tempimg.SetPixel(x, y, img.GetPixel(x + (32 * linex), y + (32 * liney)));
	//					tempimg.Apply();
	//				}
	//			}
	//			images[linex + (3 * liney)] = tempimg;
	//		}
	//	}

	//	// Création de 12 sprites répartie dans un tableau de 4 tableau de 3 sprites chacun
	//	for (int y = 0; y < 4; y++)
	//	{
	//		Sprite[] tempSprites = new Sprite[3];
	//		for (int x = 0; x < 3; x++)
	//		{
	//			tempSprites[x] = Sprite.Create(images[x+(y*3)], new Rect(0f, 0f, images[x + (y*3)].width/*Taille width*/, images[x + (y*3)].height/*Taille height*/), new Vector2(0.5f, 0.5f), 30);
	//			spriteRenderer.sprite = tempSprites[x];
	//		}
	//		sprites.Add(tempSprites);
	//	}

	//	// Liste de sprite de déplacement vers la droite
	//	Sprite[] spritesForAnimation = sprites[1];
	//	animClip = new AnimationClip();
	//	animClip.frameRate = 12;   // FPS
	//	animClip.name = "walk";
	//	animClip.wrapMode = WrapMode.Loop;
	//	EditorCurveBinding spriteBinding = new EditorCurveBinding();
	//	spriteBinding.type = typeof(SpriteRenderer);
	//	spriteBinding.path = "";
	//	spriteBinding.propertyName = "m_Sprite";
	//	ObjectReferenceKeyframe[] spriteKeyFrames = new ObjectReferenceKeyframe[spritesForAnimation.Length];
	//	for (int i = 0; i < (spritesForAnimation.Length); i++)
	//	{
	//		spriteKeyFrames[i] = new ObjectReferenceKeyframe();
	//		spriteKeyFrames[i].time = i / animClip.frameRate;
	//		spriteKeyFrames[i].value = spritesForAnimation[i];
	//	}
	//	AnimationUtility.SetObjectReferenceCurve(animClip, spriteBinding, spriteKeyFrames);

	//	animation.AddClip(animClip, "walk");
	//	animation.Play("walk");
	//	Debug.Log(animation.GetClip("walk").name);

	//	// Sauvegarde de l'animation
	//	AssetDatabase.CreateAsset(animClip, "Assets/Profiles/Mathis/Animations/walk.anim");
	//	AssetDatabase.SaveAssets();
	//	AssetDatabase.Refresh();

	//	spriteRenderer.sprite = sprites[1][0];
	//}
}
