using System;

[Serializable]
public class AzureFaceResponse
{
	public AzureFace[] faceList;
}

[Serializable]
public class AzureFace
{
    public string faceId;
    public FaceRectangle faceRectangle;
	public FaceAttributes faceAttributes;
	public FaceLandmarks faceLandmarks;
}

[Serializable]
public class FaceRectangle
{
    public int top;
	public int left;
	public int width;
	public int height;
}

[Serializable]
public class FaceAttributes
{
	public Emotion emotion;
}

[Serializable]
public class Emotion
{
	public float anger;
	public float contempt;
	public float disgust;
	public float fear;
	public float happiness;
	public float neutral;
	public float sadness;
	public float surprise;
}

[Serializable]
public class FaceLandmarks
{
	public Landmark pupilRight;
	public Landmark pupilLeft;
}

[Serializable]
public class Landmark
{
	public float x;
	public float y;
}

/* 
{
	"faceList":[
		{
			"faceId":"fcbb21d9-df63-4431-9d1e-26ff1eafe3cb",
			"faceRectangle":{
				"top":174,
				"left":247,
				"width":246,
				"height":246
			},
			"faceAttributes":{
				"emotion":{
					"anger":0.0,
					"contempt":0.0,
					"disgust":0.0,
					"fear":0.0,
					"happiness":0.983,
					"neutral":0.017,
					"sadness":0.0,
					"surprise":0.0
				}
			}
		}
	]
}
*/