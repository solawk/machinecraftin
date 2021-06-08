// Friday Night Machinecraftin' script - version 08.06-13:30
// Created by Alexander Khanov a.k.a. Solawk

// Friday Night Funkin' by ninja_muffin99
// Music tracks by their respective composers

// Just put the parsed chart into the designated line below and change the name in GetUserData (line 165) to yours
// Refer to README.md for troubleshooting

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class FNF : UserScript
{
	// Chart
	List<int> Indexes = new List<int>();
	List<int> Times = new List<int>();
	List<int> Durations = new List<int>();
	int noteCount = 0;
	private void ImportChart()
	{
		// vvv Drop the parser output here vvv
		
		// ^^^ Drop the parser output here ^^^
		
		scrollDistancePerSecond = 640 * (scrollSpeed / 1.5f);
		safespaceDistance = scrollDistancePerSecond * (safetimeMs / 1000);
		
		readyTime = 4f / scrollSpeed;
		readyTimer = readyTime;
		
		bpmTime = 60f / BPM;
		
		NoteIndex = new int[noteCount];
		NoteTime = new int[noteCount];
		NoteDuration = new int[noteCount];	
		
		for (int i = 0; i < noteCount; i++)
		{
			NoteIndex[i] = Indexes[i];
			NoteTime[i] = Times[i];
			NoteDuration[i] = Durations[i];
		}
	}	
	private void AddNote(int index, int time, int duration)
	{
		noteCount++;
		Indexes.Add(index);
		Times.Add(time);
		Durations.Add(duration);
	}	
	
	// Notes
	int[] NoteIndex;
	int[] NoteTime;
	int[] NoteDuration;
	int nextNoteToSpawn = 0;
	
	// Audio files
	string SongName = "";
	int MissVolume = 10;
	string MissName = "missnote";
	float missCooldown = 0;
	int sustainer = 10;
	bool play = false;
	
	bool[] Pressed = { false, false, false, false };
	bool[] CanBePressed = { true, true, true, true };
	bool[] Held = { false, false, false, false };
	string[] Names = { "left", "down", "up", "right" };
	KeyCode[] Keys = { KeyCode.A, KeyCode.S, KeyCode.UpArrow, KeyCode.RightArrow };
	
	// Arrow animations
	bool[] WasConfirmed = { false, false, false, false };
	int[] AnimationSteps = { 0, 0, 0, 0 };
	
	string[] GoalArrowNames = { "FNF/FNF_emptyNoteLeft", "FNF/FNF_emptyNoteDown", "FNF/FNF_emptyNoteUp", "FNF/FNF_emptyNoteRight" };
	string[] DullArrowNames = { "FNF/FNF_dullNoteLeft", "FNF/FNF_dullNoteDown", "FNF/FNF_dullNoteUp", "FNF/FNF_dullNoteRight" };
	string[] ConfirmedArrowNames = { "FNF/FNF_confNoteLeft", "FNF/FNF_confNoteDown", "FNF/FNF_confNoteUp", "FNF/FNF_confNoteRight" };
	string[] ArrowNames = { "FNF/FNF_noteLeft", "FNF/FNF_noteDown", "FNF/FNF_noteUp", "FNF/FNF_noteRight" };
	string[] StretchNames = { "FNF/FNF_stretchLeft", "FNF/FNF_stretchDown", "FNF/FNF_stretchUp", "FNF/FNF_stretchRight" };
	string[] StretchEndNames = { "FNF/FNF_stretchEndLeft", "FNF/FNF_stretchEndDown", "FNF/FNF_stretchEndUp", "FNF/FNF_stretchEndRight" };
	int StretchLength = 28;
	
	string[] ReadySetGoNames = { "FNF/FNF_ready", "FNF/FNF_set", "FNF/FNF_go" };
	string[] ReadySetGoSoundsNames = { "intro3", "intro2", "intro1", "introGo" };
	float readyTime = 0f;
	float readyTimer = 0f;
	List<GameObject> Readies = new List<GameObject>();
	
	Dictionary<string, Sprite> Sprites = new Dictionary<string, Sprite>();
	
	// Scale all imagery accroding to the screen size
	float ScaleFactor = 1;
	
	string[] RightActionNames = { "RightL", "RightD", "RightU", "RightR" };
	string[] LeftActionNames = { "LeftL", "LeftD", "LeftU", "LeftR" };
	
	int goalY = 250;
	int[] goalX = { 150, 260, 370, 480, -480, -370, -260, -150 };
	
	static float scrollSpeed = 0f;
	static float safetimeMs = 100;
	
	float scrollDistancePerSecond = 0f;
	float offsetMs = 100;
	float safespaceDistance = 0f;
	
	float previousTime = Time.unscaledTime;
	float startTime = 0;
	
	int misses = 0;
	int score = 0;
	
	Dictionary<string, GameObject> Objects = new Dictionary<string, GameObject>();
	
	// Decided to pool arrows too
	static int arrowsMax = 100;
	GameObject[] Arrows = new GameObject[arrowsMax];
	int arrowsInUse = 0;
	
	// Stretches use a pre-made pool for optimization
	static int stretchesMax = 200;
	GameObject[] Stretches = new GameObject[stretchesMax];
	int stretchesInUse = 0;
	
	// Pool for the combo counter (cc)
	static int ccMax = 90;
	GameObject[] CC = new GameObject[ccMax];
	float[] CCspeeds = new float[ccMax];
	int ccInUse = 0;
	string ccName = "FNF/FNF_num";
	int comboY = 0;
	int comboBlastY = -50;
	
	int COMBO = 0;
	
	// Press quality shoutouts pooling
	static int shoutoutsMax = 20;
	GameObject[] Shoutouts = new GameObject[shoutoutsMax];
	float[] ShoutoutsSpeeds = new float[shoutoutsMax];
	int shoutoutsInUse = 0;
	int shoutoutY = 150;
	int shoutoutBlastY = 100;
	string[] ShoutoutNames = { "FNF/FNF_shit", "FNF/FNF_bad", "FNF/FNF_good", "FNF/FNF_sick" };
	
	bool SYNC1 = false;
	bool SYNC2 = false;
	bool dead = false;
	bool stretchesPooled = false;
	
	int bpmMeter = 0;
	static int BPM = 0;
	float bpmTime = 0f;
	float bpmTimer = 621f;
	
	int slowFrames = 0;
	float lastSlowFrameTime = 0f;
	
	float[] LastFrameTimes = new float[60];
	
	public override string GetUserName()
	{
		return "Put your name on line 165";
	}

	public override void OnStart(AutoPilot ap)
	{
		var scaleTester = ap.CreateSprite(GoalArrowNames[0]);
		ScaleFactor = scaleTester.GetComponentInParent<Canvas>().pixelRect.width / 1920;
		Destroy(scaleTester);
		
		for (int a = 0; a < 4; a++)
		{
			Objects.Add(Names[a] + "ArrowOpp", ap.CreateSprite(GoalArrowNames[a]));
			Objects[Names[a] + "ArrowOpp"].GetComponent<RectTransform>().anchoredPosition = new Vector2(goalX[a + 4], goalY);
			Objects[Names[a] + "ArrowOpp"].GetComponent<RectTransform>().localScale = new Vector3(ScaleFactor, ScaleFactor, 1);			
		}
		
		// Player's arrows
			
		for (int a = 0; a < 4; a++)
		{
			Objects.Add(Names[a] + "Arrow", ap.CreateSprite(GoalArrowNames[a]));
			Sprites.Add(Names[a] + "ArrowEmpty", Objects[Names[a] + "Arrow"].GetComponent<Image>().sprite);
			Objects[Names[a] + "Arrow"].GetComponent<RectTransform>().anchoredPosition = new Vector2(goalX[a], goalY);
			Objects[Names[a] + "Arrow"].GetComponent<RectTransform>().localScale = new Vector3(ScaleFactor, ScaleFactor, 1);
		}
		
		// Regular arrows to press
		
		for (int a = 0; a < 4; a++)
		{
			var arrow = ap.CreateSprite(ArrowNames[a]);
			Sprites.Add(Names[a] + "Arrow", arrow.GetComponent<Image>().sprite);
			Destroy(arrow);
		}
		
		// Stretches
		
		for (int a = 0; a < 4; a++)
		{
			var stretch = ap.CreateSprite(StretchNames[a]);
			Sprites.Add(Names[a] + "Stretch", stretch.GetComponent<Image>().sprite);
			Destroy(stretch);
			
			var stretchEnd = ap.CreateSprite(StretchEndNames[a]);
			Sprites.Add(Names[a] + "StretchEnd", stretchEnd.GetComponent<Image>().sprite);
			Destroy(stretchEnd);
		}
		
		// Combo counter digits
		
		for (int n = 0; n < 10; n++)
		{
			var digit = ap.CreateSprite(ccName + n.ToString());
			Sprites.Add("digit" + n.ToString(), digit.GetComponent<Image>().sprite);
			Destroy(digit);
		}
		
		// Shoutouts
		
		for (int q = 0; q < 4; q++)
		{
			var shoutout = ap.CreateSprite(ShoutoutNames[q]);
			Sprites.Add("shoutout" + q.ToString(), shoutout.GetComponent<Image>().sprite);
			Destroy(shoutout);
		}
		
		// Arrows' frames
		
		for (int a = 0; a < 4; a++)
		{
			for (int i = 1; i <= 3; i++)
			{
				string arrowName = Names[a] + "Arrow";
				
				var dull = ap.CreateSprite(DullArrowNames[a] + i.ToString());
				var conf = ap.CreateSprite(ConfirmedArrowNames[a] + i.ToString());
				
				Sprites.Add(arrowName + "Dull" + i.ToString(), dull.GetComponent<Image>().sprite);
				Sprites.Add(arrowName + "Conf" + i.ToString(), conf.GetComponent<Image>().sprite);
				
				Destroy(dull);
				Destroy(conf);
			}
		}
		
		PoolStretches(ap);
		PoolArrows(ap);
		PoolCC(ap);
		PoolShoutouts(ap);
		
		ImportChart();		
	}

	public override void OnUpdate(AutoPilot ap)
	{
		if (!stretchesPooled)
		{
			stretchesPooled = true;
			
		}
		
		for (int i = 0; i < 4; i++)
		{
			CheckInput(Keys[i], i, ap);
			ap.Print(i, Names[i] + ": " + Pressed[i] + ", " + Held[i]);
		}
		ap.Print(5, "Misses: " + misses.ToString());
		ap.Print(6, "Score: " + score.ToString());
		InputActions(ap);
		
		float deltaTime = Time.unscaledTime - previousTime;
		
		// Do stuff
		
		MoveArrows(deltaTime, ap);
		MoveShoutouts(deltaTime);
		MoveCC(deltaTime);
		DissolveReadies(deltaTime);
		if (play)
		{		
			Spawner(ap);
		}
		
		// FPS counting
		for (int i = 1; i < 60; i++)
		{
			LastFrameTimes[i - 1] = LastFrameTimes[i];
		}
		LastFrameTimes[59] = deltaTime;
		float fps = 0f;
		for (int i = 0; i < 60; i++)
		{
			fps += LastFrameTimes[i];
		}
		ap.Print(7, "FPS: " + (60f / fps).ToString());
		if (deltaTime > 0.033f)
		{
			slowFrames++;
			lastSlowFrameTime = deltaTime;
		}
		ap.Print(8, "Slow frames: " + slowFrames);
		ap.Print(9, "LSFT: " + lastSlowFrameTime);
		
		// Stop doing stuff
		// it's okay i am stuff
		// omg machinecraft nooooo
		// haha boyfriender you are banging my OnUpdaughter
				
		previousTime = Time.unscaledTime;		
		Unpress();		
		
		// Music
		if (play)
		{
			if (Time.unscaledTime - startTime >= 0f)
			{			
				ap.PlaySoundLoop(SongName + "_Inst", 100, 100);
			
				if (missCooldown <= 0 || sustainer <= 0)
				{				
					ap.PlaySoundLoop(SongName + "_Voices", 100, 100);
					sustainer = 10;
				}
				else
				{
					missCooldown -= deltaTime * 1000;
					sustainer--;
				}
				
				if (SYNC1 && !SYNC2)
				{
					SYNC2 = true;
					startTime = Time.unscaledTime - 0.016f;
					bpmTimer = bpmTime;
				}
				
				SYNC1 = true;
			}
		}
		else
		{
			if (Input.GetKey(KeyCode.Space) && !play && !dead)
			{				
				startTime = Time.unscaledTime + readyTime;				
				play = true;
			}
		}
		
		// Ready set go
		if (play && readyTimer > 0f)
		{
			float prevTime = readyTimer;
			readyTimer -= deltaTime;
			
			// 3
			if (prevTime >= readyTime && readyTimer < readyTime)
			{
				ap.PlaySound2D(ReadySetGoSoundsNames[0], 50);
				AddReady(0, ap);
			}
			
			// 2
			if (prevTime >= readyTime * 0.75f && readyTimer < readyTime * 0.75f)
			{
				ap.PlaySound2D(ReadySetGoSoundsNames[1], 50);
				AddReady(1, ap);
			}
			
			// 1
			if (prevTime >= readyTime * 0.5f && readyTimer < readyTime * 0.5f)
			{
				ap.PlaySound2D(ReadySetGoSoundsNames[2], 50);
				AddReady(2, ap);
			}
			
			// Go
			if (prevTime >= readyTime * 0.25f && readyTimer < readyTime * 0.25f)
			{
				ap.PlaySound2D(ReadySetGoSoundsNames[3], 50);
				AddReady(3, ap);
			}
		}
		
		// bpm
		if (play)
		{
			bpmTimer -= deltaTime;
			if (bpmTimer <= 0)
			{
				bpmTimer += bpmTime;
				
				switch (bpmMeter)
				{
					case 0:
					{
						ap.StartAction("BeatL", 1000);
						break;
					}
					
					case 1:
					{
						ap.EndAction("BeatL");
						break;
					}
					
					case 2:
					{
						ap.StartAction("BeatR", 1000);
						break;
					}
					
					case 3:
					{
						ap.EndAction("BeatR");
						break;
					}
				}
				
				bpmMeter = (bpmMeter + 1) % 4;
			}
		}
		
		// Death
		if (Input.GetKey(KeyCode.R) && play)
		{
			play = false;
			dead = true;
		}
	}
	
	// Check the input for the lane, determine is it pressed or held or sth
	private void CheckInput(KeyCode key, int index, AutoPilot ap)
	{
		if (Input.GetKey(key))
		{
			if (!Pressed[index] && CanBePressed[index])
			{
				Pressed[index] = true;
				CanBePressed[index] = false;
				
				// Press actions
				PressArrow(index, ap);
			}
			
			Held[index] = true;
			
			// Hold actions
			HoldStretch(index, ap);
		}
		else
		{
			Held[index] = false;
			CanBePressed[index] = true;
		}
		
		ProgressPressAnimation(index);
	}
	
	// Try to press an arrow
	private void PressArrow(int index, AutoPilot ap)
	{
		GameObject nearestFittingArrow = null;
		int nearestFittingArrowIndex = -1;
		float nearestFittingArrowDistance = 9999;
		
		for (int i = 0; i < arrowsInUse; i++)
		{
			GameObject arrow = Arrows[i];
			
			// If fitting
			if (Mathf.Abs(arrow.GetComponent<RectTransform>().anchoredPosition.x - goalX[index]) < 1f)
			{
				float distance = Mathf.Abs(arrow.GetComponent<RectTransform>().anchoredPosition.y - goalY);
				if (nearestFittingArrowDistance > distance)
				{
					nearestFittingArrow = arrow;
					nearestFittingArrowIndex = i;
					nearestFittingArrowDistance = distance;
				}
			}
		}
		
		if (!nearestFittingArrow)
		{
			// Miss when the lane is empty
			misses++;
			NullCombo(ap);
			ap.PlaySound2D(MissName + Mathf.FloorToInt(Random.Range(1, 4)), MissVolume);
		}
		else
		{
			if (nearestFittingArrowDistance > safespaceDistance)
			{
				// Miss when the lane is not empty
				misses++;
				NullCombo(ap);
				score -= 10;
				missCooldown = 200;
				ap.PlaySound2D(MissName + Mathf.FloorToInt(Random.Range(1, 4)), MissVolume);
				return;
			}
			
			WasConfirmed[index] = true;
			RetractArrow(nearestFittingArrowIndex);
			
			if (nearestFittingArrowDistance > safespaceDistance * 0.9f)
			{
				// Shit
				DeployShoutout(0, ap);
				COMBO++;
				DeployCombo(ap);
				return;
			}
			
			if (nearestFittingArrowDistance > safespaceDistance * 0.6f)
			{
				// Bad
				score += 50;
				DeployShoutout(1, ap);
				COMBO++;
				DeployCombo(ap);
				return;
			}
			
			if (nearestFittingArrowDistance > safespaceDistance * 0.3f)
			{
				// Good
				score += 200;
				DeployShoutout(2, ap);
				COMBO++;
				DeployCombo(ap);
				return;
			}
			
			// Sick
			score += 350;
			DeployShoutout(3, ap);
			COMBO++;
			DeployCombo(ap);
			return;
		}
	}
	
	private void NullCombo(AutoPilot ap)
	{
		if (COMBO > 0)
		{
			if (COMBO < 10)
			{
				COMBO = 0;
				return;
			}
			
			COMBO = 0;
			DeployCombo(ap);
		}	
	}
	
	// Create combo when needed
	private void DeployCombo(AutoPilot ap)
	{
		if (COMBO > 0 && COMBO < 10) return;
		
		int hundreds = Mathf.FloorToInt(COMBO / 100);
		int tens = Mathf.FloorToInt(COMBO / 10) - (hundreds * 10);
		int ones = Mathf.FloorToInt(COMBO % 10);
		
		DeployCC(-45, hundreds, ap);
		DeployCC(0, tens, ap);
		DeployCC(45, ones, ap);
	}
	
	// Try to hold a stretch
	private void HoldStretch(int index, AutoPilot ap)
	{
		GameObject nearestFittingStretch = null;
		int nearestFittingStretchIndex = -1;
		float nearestFittingStretchDistance = 9999;
		
		for (int i = 0; i < stretchesInUse; i++)
		{
			GameObject stretch = Stretches[i];
			
			// If fitting
			if (Mathf.Abs(stretch.GetComponent<RectTransform>().anchoredPosition.x - goalX[index]) < 1f)
			{
				float distance = Mathf.Abs(stretch.GetComponent<RectTransform>().anchoredPosition.y - goalY);
				if (nearestFittingStretchDistance > distance)
				{
					nearestFittingStretch = stretch;
					nearestFittingStretchIndex = i;
					nearestFittingStretchDistance = distance;
				}
			}
		}
		
		if (!nearestFittingStretch)
		{
			// Just holdin' aroung
		}
		else
		{
			if (nearestFittingStretchDistance > StretchLength)
			{
				// Just holdin' but it's not yet there
				return;
			}
			
			// Stretch connected
			WasConfirmed[index] = true;
			AnimationSteps[index] = 0;
			score += 10;
			RetractStretch(nearestFittingStretchIndex);
			return;
		}
	}
	
	// Progress animations while the arrow is held
	private void ProgressPressAnimation(int index)
	{
		if (Held[index])
		{
			if (++AnimationSteps[index] > 3)
			{
				if (WasConfirmed[index])
				{
					WasConfirmed[index] = false;
				}
				
				AnimationSteps[index] = 3;
			}
			
			SetPressAnimationStep(index);
		}
		else if (AnimationSteps[index] > 0)
		{
			ClearPressAnimation(index);
		}
	}
	
	// Set animation step
	private void SetPressAnimationStep(int index)
	{		
		Sprite Frame = null;
		float size = 1f;
		
		if (WasConfirmed[index])
		{
			Frame = Sprites[Names[index] + "ArrowConf" + AnimationSteps[index].ToString()];
			size = 1.4f;
		}
		else
		{
			Frame = Sprites[Names[index] + "ArrowDull" + AnimationSteps[index].ToString()];
		}
			
		// Put the needed frame in place
		Objects[Names[index] + "Arrow"].GetComponent<Image>().sprite = Frame;
		Objects[Names[index] + "Arrow"].GetComponent<RectTransform>().localScale = new Vector3(ScaleFactor * size, ScaleFactor * size, 1);
	}
	
	// Clear the press animation
	private void ClearPressAnimation(int index)
	{
		AnimationSteps[index] = 0;
		
		// Set the default arrow
		Objects[Names[index] + "Arrow"].GetComponent<Image>().sprite = Sprites[Names[index] + "ArrowEmpty"];
		Objects[Names[index] + "Arrow"].GetComponent<RectTransform>().localScale = new Vector3(ScaleFactor, ScaleFactor, 1);
	}
	
	// Unpress all arrows after their checks have been done
	private void Unpress()
	{
		for (int i = 0; i < 4; i++)
		{
			Pressed[i] = false;
		}
	}
	
	// Send the actions to the machine. Have you forgotten it's still MachineCraft?
	private void InputActions(AutoPilot ap)
	{
		for (int i = 0; i < 4; i++)
		{
			if (Held[i])
			{
				ap.StartAction(RightActionNames[i], 1);
			}
			else
			{
				ap.EndAction(RightActionNames[i]);
			}
		}
	}
	
	// Spawns the notes from the chart
	private void Spawner(AutoPilot ap)
	{
		if (nextNoteToSpawn == noteCount) return;
		
		float msPassed = (Time.unscaledTime - startTime) * 1000;
		float spawnOffsetMs = (960 / scrollDistancePerSecond) * 1000;
			
		int nextNoteTime = NoteTime[nextNoteToSpawn];
		while (nextNoteTime <= msPassed + spawnOffsetMs - offsetMs)
		{
			AddArrow(NoteIndex[nextNoteToSpawn], NoteDuration[nextNoteToSpawn], ap);
			
			if (++nextNoteToSpawn == noteCount) return;
			
			nextNoteTime = NoteTime[nextNoteToSpawn];
		}
	}
	
	// Spawn an arrow, optionally - with a stretch
	private void AddArrow(int index, int duration, AutoPilot ap)
	{
		// Spawning the stretch first
		int stretchTotalLength = Mathf.RoundToInt((scrollDistancePerSecond / 1000) * duration);
		int stretchesNeeded = Mathf.FloorToInt(stretchTotalLength / StretchLength);
		for (int i = 0; i < stretchesNeeded; i++)
		{
			if (i == stretchesNeeded - 1)
			{
				// Spawn the ending
				DeployStretch(index, goalY - 960 - (StretchLength * 0.27f) - (StretchLength * (i + 0.5f)), true, ap);
			}
			else
			{
				// Spawn the part
				DeployStretch(index, goalY - 960 - (StretchLength * 0.27f) - (StretchLength * (i + 0.5f)), false, ap);
			}		
		}
		
		DeployArrow(index, goalY - 960, ap);
	}
	
	// Move all arrows and stretches upwards, deleting the missed ones
	private void MoveArrows(float delta, AutoPilot ap)
	{
		float scrollDistance = scrollDistancePerSecond * delta;
		
		for (int i = 0; i < arrowsInUse; i++)
		{
			GameObject arrow = Arrows[i];
			Vector2 arrowVector = arrow.GetComponent<RectTransform>().anchoredPosition;
			
			// Destroy the opponent's arrows
			if (arrowVector.x < 0 && arrowVector.y >= goalY)
			{
				OpponentAction(arrowVector.x, ap);
				RetractArrow(i);
				continue;
			}
			
			arrow.GetComponent<RectTransform>().anchoredPosition = arrowVector + Vector2.up * scrollDistance;

			// Miss
			if (arrowVector.y > 450)
			{
				misses++;
				NullCombo(ap);
				RetractArrow(i);
			}	
		}
		
		for (int i = 0; i < stretchesInUse; i++)
		{
			GameObject stretch = Stretches[i];
			Vector2 stretchVector = stretch.GetComponent<RectTransform>().anchoredPosition;
						
			// Destroy the opponent's stretches
			if (stretchVector.x < 0 && stretchVector.y + StretchLength >= goalY)
			{
				OpponentAction(stretchVector.x, ap);
				RetractStretch(i);
				continue;
			}

			stretch.GetComponent<RectTransform>().anchoredPosition = stretchVector + Vector2.up * scrollDistance;
			
			// Miss
			if (stretchVector.y > 450)
			{
				RetractStretch(i);
			}		
		}
	}
	
	// Create arrowsMax arrows beforehand
	private void PoolArrows(AutoPilot ap)
	{
		for (int i = 0; i < arrowsMax; i++)
		{
			GameObject newArrow = ap.CreateSprite(GoalArrowNames[0]);
			newArrow.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -10000f);
			newArrow.GetComponent<RectTransform>().localScale = new Vector3(ScaleFactor, ScaleFactor, 1);
			Arrows[i] = newArrow;
		}		
	}
	
	// Deploy an arrow to the position
	private void DeployArrow(int lane, float y, AutoPilot ap)
	{
		if (arrowsInUse == arrowsMax)
		{
			ap.Log("Not enough arrows!!");
			return;
		}
		
		var arrow = Arrows[arrowsInUse++];
		arrow.GetComponent<RectTransform>().anchoredPosition = new Vector2(goalX[lane], y);
		arrow.GetComponent<Image>().sprite = Sprites[Names[lane % 4] + "Arrow"];
		
	}
	
	// Retract an arrow to safety after it's used
	private void RetractArrow(int index)
	{
		var arrow = Arrows[index];
		for (int i = index + 1; i < arrowsMax; i++)
		{
			Arrows[i - 1] = Arrows[i];
		}
		Arrows[arrowsMax - 1] = arrow;
		arrowsInUse--;
		arrow.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -10000f);
	}
	
	// Create stretchesMax stretches beforehand
	private void PoolStretches(AutoPilot ap)
	{
		for (int i = 0; i < stretchesMax; i++)
		{
			GameObject newStretch = ap.CreateSprite(GoalArrowNames[0]);
			newStretch.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -10000f);
			newStretch.GetComponent<RectTransform>().localScale = new Vector3(ScaleFactor * 0.275f, ScaleFactor * 0.275f, 1);
			Stretches[i] = newStretch;
		}		
	}
	
	// Deploy a stretch to the position
	private void DeployStretch(int lane, float y, bool isEnd, AutoPilot ap)
	{
		if (stretchesInUse == stretchesMax)
		{
			ap.Log("Not enough stretches!!");
			return;
		}
		
		var stretch = Stretches[stretchesInUse++];
		stretch.GetComponent<RectTransform>().anchoredPosition = new Vector2(goalX[lane], y);
		stretch.GetComponent<Image>().sprite = Sprites[Names[lane % 4] + "Stretch" + (isEnd ? "End" : "")];
		
	}
	
	// Retract a stretch to safety after it's used
	private void RetractStretch(int index)
	{
		var stretch = Stretches[index];
		for (int i = index + 1; i < stretchesMax; i++)
		{
			Stretches[i - 1] = Stretches[i];
		}
		Stretches[stretchesMax - 1] = stretch;
		stretchesInUse--;
		stretch.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -10000f);
	}
	
	// Create ccMax digits beforehand
	private void PoolCC(AutoPilot ap)
	{
		for (int i = 0; i < ccMax; i++)
		{
			GameObject newCC = ap.CreateSprite(GoalArrowNames[0]);
			newCC.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0f, -10000f, 0f);
			newCC.GetComponent<RectTransform>().localScale = new Vector3(ScaleFactor * 0.5f, ScaleFactor * 0.6f, 1);
			CC[i] = newCC;
		}
	}
	
	// Deploy a CC digit to the position
	private void DeployCC(int x, int digit, AutoPilot ap)
	{
		if (ccInUse == ccMax)
		{
			ap.Log("Not enough combo digits!!");
			return;
		}
		
		var cc = CC[ccInUse];
		CCspeeds[ccInUse++] = Random.Range(200f, 220f);
		cc.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(x, comboY, -10000f);
		cc.GetComponent<Image>().sprite = Sprites["digit" + digit.ToString()];		
	}
	
	// Retract an digit to safety after it's used
	private void RetractCC(int index)
	{
		var cc = CC[index];
		for (int i = index + 1; i < ccMax; i++)
		{
			CC[i - 1] = CC[i];
			CCspeeds[i - 1] = CCspeeds[i];
		}
		CC[ccMax - 1] = cc;
		CCspeeds[ccMax - 1] = 0;
		ccInUse--;
		cc.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0f, -10000f, 0f);
	}
	
	private void MoveCC(float delta)
	{
		for (int i = 0; i < ccInUse; i++)
		{
			GameObject cc = CC[i];
			Vector3 ccVector = cc.GetComponent<RectTransform>().anchoredPosition;
			cc.GetComponent<RectTransform>().anchoredPosition3D = ccVector + Vector3.up * CCspeeds[i] * delta + new Vector3(0, 0, 1f);
			CCspeeds[i] -= 500f * delta;
			
			float alpha = CCspeeds[i] >= 0 ? 1f : 1f - (CCspeeds[i] / -200f);
			cc.GetComponent<Image>().color = new Color(1f, 1f, 1f, alpha);
			
			// Blast
			if (cc.GetComponent<RectTransform>().anchoredPosition.y < comboBlastY)
			{
				RetractCC(i);
			}	
		}
	}
	
	private void OpponentAction(float objectX, AutoPilot ap)
	{
		for (int lane = 0; lane < 4; lane++)
		{
			if (Mathf.Abs(objectX - goalX[lane + 4]) < 1f)
			{
				// Start action of the opponent
				ap.StartAction(LeftActionNames[lane], 10);
			}
		}
	}
	
	// Create shoutoutsMax shoutouts beforehand
	private void PoolShoutouts(AutoPilot ap)
	{
		for (int i = 0; i < shoutoutsMax; i++)
		{
			GameObject newShoutout = ap.CreateSprite(GoalArrowNames[0]);
			newShoutout.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0f, -10000f, 0f);
			newShoutout.GetComponent<RectTransform>().localScale = new Vector3(ScaleFactor * 2.4f, ScaleFactor, 1);
			Shoutouts[i] = newShoutout;
		}		
	}
	
	// Deploy a shoutout to the position
	private void DeployShoutout(int quality, AutoPilot ap)
	{
		if (shoutoutsInUse == shoutoutsMax)
		{
			ap.Log("Not enough shoutouts!!");
			return;
		}
		
		var shoutout = Shoutouts[shoutoutsInUse];
		ShoutoutsSpeeds[shoutoutsInUse++] = 150f;
		shoutout.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, shoutoutY, -10000f);
		shoutout.GetComponent<Image>().sprite = Sprites["shoutout" + quality.ToString()];		
	}
	
	// Retract an shoutout to safety after it's used
	private void RetractShoutout(int index)
	{
		var shoutout = Shoutouts[index];
		for (int i = index + 1; i < shoutoutsMax; i++)
		{
			Shoutouts[i - 1] = Shoutouts[i];
			ShoutoutsSpeeds[i - 1] = ShoutoutsSpeeds[i];
		}
		Shoutouts[shoutoutsMax - 1] = shoutout;
		ShoutoutsSpeeds[shoutoutsMax - 1] = 0;
		shoutoutsInUse--;
		shoutout.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0f, -10000f, 0f);
	}
	
	private void MoveShoutouts(float delta)
	{
		for (int i = 0; i < shoutoutsInUse; i++)
		{
			GameObject shoutout = Shoutouts[i];
			Vector3 shoutoutVector = shoutout.GetComponent<RectTransform>().anchoredPosition;
			shoutout.GetComponent<RectTransform>().anchoredPosition3D = shoutoutVector + Vector3.up * ShoutoutsSpeeds[i] * delta + new Vector3(0, 0, 1f);
			ShoutoutsSpeeds[i] -= 1000f * delta;
			
			float alpha = ShoutoutsSpeeds[i] >= 0 ? 1f : 1f - (ShoutoutsSpeeds[i] / -300f);
			shoutout.GetComponent<Image>().color = new Color(1f, 1f, 1f, alpha);
			
			// Blast
			if (shoutout.GetComponent<RectTransform>().anchoredPosition.y < shoutoutBlastY)
			{
				RetractShoutout(i);
			}	
		}
	}
	
	private void AddReady(int state, AutoPilot ap)
	{
		if (state == 0) return;
		
		GameObject newReady = ap.CreateSprite(ReadySetGoNames[state - 1]);
		newReady.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
		newReady.GetComponent<RectTransform>().localScale = new Vector3(ScaleFactor, ScaleFactor, 1);
			
		Readies.Add(newReady);
	}
	
	private void DissolveReadies(float delta)
	{
		for (int i = Readies.Count - 1; i >= 0; i--)
		{
			GameObject ready = Readies[i];
			
			Color prevColor = ready.GetComponent<Image>().color;
			float dissolveSpeed = 2f;
			ready.GetComponent<Image>().color = new Color(prevColor.r, prevColor.g, prevColor.b, prevColor.a - delta * dissolveSpeed);
			
			// Destroy
			if (ready.GetComponent<Image>().color.a <= 0f)
			{
				RemoveReady(i);
			}	
		}
	}
	
	private void RemoveReady(int index)
	{
		Destroy(Readies[index]);
		Readies.RemoveAt(index);
	}
}
