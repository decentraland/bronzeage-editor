using UnityEngine;
using System.Collections;

public class uJet_Jetpack_Controller : MonoBehaviour {


	[Space(15)]
	[Tooltip("Set this True to see the instruction in your Console, upon entering the Play Mode in editor! If something is wrong, turning this True should show you the solution in most common cases.")]
	public bool ShowInstructions;
	[Space(15)]
	[Tooltip("Please, specify the Rigidbody Component, which will be used for flying.")]
	public Rigidbody Rig;
	[Tooltip("So what's the acceleration speed of this object, when input is pressed?   [INFO:] The Throttle power is lowered accordingly, when more than one move input button is pressed - to eliminate the so called Diagonal Speed Boost problem, common in older games. Feel free to Google it up.")]
	public float Throttle = 40f;
	[Tooltip("You can set the gravity force, independent from the Rigidbody's Gravity setting. It's good to use if your rigidbody falls too slow or too fast. [WARNING!] This function sums up with the default Rigidbody Gravity, so it might be wise to use only one of these two - the Rigidbody gravity, or this script's FallSpeed. If you wish to turn FallSpeed off, simply set it's value to 0.")]
	public float FallSpeed = 50;
	[Tooltip("And how about the max speed? An object will never go faster than this value states")]
	public float MaxSpeed = 150f;
	[Tooltip("Should the SideMove Input Axis rotate this object, or move it left/right? Set True, is it should only rotate this object (like a normal jetpack in 3D enviroment). Set False if you want it to move sideways (for 2D or when rotation is handled by other means, like mouse input).")]
	public bool SideMoveRotates;
	[Tooltip("If the above is True, what's the speed at which the object rotates?")]
	public float RotationSpeed =1;
	[Tooltip("For 2D games it's a good idea to disable one of the 3 axis of movement. This switch disables the FrontMove input.")]
	public bool DisableForwardMove;
	[Tooltip("For 2D games it's a good idea to disable one of the 3 axis of movement. This switch disables the SideMove input.")]
	public bool DisableSideMove;
	[Tooltip("For 2D games it's a good idea to disable one of the 3 axis of movement. This switch disables the VerticalMove input.")]
	public bool DisableVerticalMove;
	[Tooltip("What should be the Drag (breaking force/air resistance/slowing down), when there's no input? (Set it higher than the below Dynamic Drag if you want the player to slow down quicker when he's not pushing any buttons).")]
	public float StaticDrag =1.5f;
    [Tooltip("What should be the Drag ((breaking force/air resistance), when the Player is making input? This might lower the maximum speed of the object if set too high. [ADVICE] Keeping it below 1 is a good idea in most cases.)")]
	public float DynamicDrag = 0.3f;
	[Tooltip("If you want to use any sounds, put the AudioSource here!")]
	public AudioSource SoundSource;
	[Tooltip("Is there a sound for basic throttle (loop)?")]
	public AudioClip ThrottleSound;
	[Tooltip("Is there a sound for this object being idle (like an engine humming)?")]
	public AudioClip IdleSound;
	[Tooltip("You can set a spot shadow to be cast below the character, independently from the lighting. (It's a round shadow, which is always directly below the object. It's a common technique of aiming your landing in 3D Platformer games). [BONUS:] There are two, most commonly used spot shadows included in the package's folder for free use.")]
	public GameObject SpotShadow;
	[Tooltip("On what surfaces (Layers) should the SpotShadow be shown on (like ground layer) and which should it omit (Player model, enemies, etc.)?")] 
	public LayerMask SpotShadowSurfaces;


	[Space(15)]
	[Tooltip("An object can use the Dash mechanism. Using additional Input Button, a player can quickly change the direction he's moving in, with an additional burst of speed - like an air dodge or evasion. For this, remember to set the Input action called Dash in the Input Menager!")]
	public bool UseDash;
	[Tooltip("DashSpeed is the strength of the Dash. Don't be affraid to set it high - in most cases a value os 200-1000 is all right.")]
	public float DashSpeed = 500;
	[Tooltip("After using dash, how many seconds (or what fraction of second) have to pass before can another Dash be done?")]
	public float DashInterval = 0.2f;
	[Tooltip("Should the dash be also executed in local Vertical space of an object? (in other words: should it be able to dash up and down, in it's local space?)")] 
	public bool AllowVerticalDash = true;
	[Tooltip("Is there a sound for Dashing?")]
	public AudioClip DashSound;

	[Space(15)]
	[Header("Fuel Options")]
	[Tooltip("Should this object use Fuel for flying? Fuel supply depletes and can be recharged over time with variables below.")]
	public bool UseFuel;
	[Tooltip("If the above is True, how much fuel should this object have at max?")]
	public float MaxFuelSupply = 300;
	[Tooltip("How much fuel does this object have?")]
	public float FuelSupply = 300;
	[Tooltip("How fast should the fuel recharge?")]
	public float FuelRecharge = 1;
	[Tooltip("How much fuel (per fixedUpdate) should normal move take?")]
	public float MoveCost = 1;
	[Tooltip("If this object uses Dash, what is the cost of dashing in Fuel units?")]
	public float DashCost = 50;
	[Tooltip("Should the Fuel cost be initialized only on VerticalMove (True), or should any Axis movement cost fuel (False)?")]
	public bool FuelCostOnVerticalMove = true;
	[Tooltip("The Minimum fuel needed to move, after the fuel was completely depleted (to prevent running on 0.1 fuel for infinite amount of time)")]
	public float FuelNeeded = 70;


	private GameObject DashMarker;
	private float SideMove;
	private float ForwardMove;
	private float VerticalMove;
	private Vector3 MoveDirection;
	private Vector3 FacingDirection;
	private Vector3 SideDirection;
	private Vector3 VerticalDirection;
	private Vector3 DashDirection;
	
	private int InputCount;
	private float fullSpeed;
	private float halfSpeed;
	private float thirdSpeed;
	private float tempVelocity;
	private float tempThrottle;
	private GameObject Spot;
	private RaycastHit Spothit;
	private Vector3 Spotpoint;
	private bool DashBool;
	private bool DashReady = true;
	private bool FuelDepleted;
	private bool DrainFuel;
	private bool RechargeFuel;



	void Awake()
	{
		if (ShowInstructions) {
			Debug.Log("FOREWORD: First of all, thank you for purchasing this package! If it was any useful or simply fun to use, remember to Rate it or even leave a short review! All Assets created by Fireballed can be found here: https://www.assetstore.unity3d.com/en/#!/publisher/14546/page=1/sortby=popularity ");
			Debug.Log("INPUT: For this object to fly, you need to specify a few Input Axis Action in the Input Menager. These are: SideMove (moving/rotating left and right), ForwardMove (moving back and forth), VerticalMove (moving up and down) and Dash. For reference of how to set them, simply look at the reference picture of the Input Menager, located in the Demo Scene folder.");
			Debug.Log("RIGIDBODY: This script requires a Rigidbody to work - setting it's collisions to Continuous Dynamic is a good idea to prevent it from going through walls at high speeds. Also you can freely set it's options, like gravity, mass, freezing rotation and position. Notice: Freezing Y axis Rotation will NOT prevent the Object from rotating, when the SideMoveRotates feature is active (in this script's Inspector Menu). So it's generally good to keep the rotation frozen on all axes or to manage it with oter scripts (like the Mouselook included in the Demo Scene folder or the Unity Standard Assets one), to prevent chaotic rotations of your Object upon colisions.");  
			Debug.Log("DOCUMENTATION: All documentation is implemented into Tooltips - simply move your mouse over a variable you want to know about and you will be greeted with all instructions, hints, requirements and effects of using each variable.");
		}
	}

	void Start(){


		fullSpeed = Throttle;					
		halfSpeed = Throttle * 0.707106682f;       //this variant of Throttle is preserving it from "Diagonal Speed Boost" (cheat), common in older games. Feel free to google it up.    
														// this modifier was calculated according to algebra and geometry, lots of mathematical Mana was sacrificed while processing all the square roots. Do not change it, unless you know what you're doing, mathematicaly-wise. 	     



		if (SpotShadow != null) {
			Spot = Instantiate(SpotShadow, Rig.transform.position, Rig.transform.rotation) as GameObject;
			Spot.transform.parent = Rig.transform;
		}

		if (UseDash == true) {
			DashMarker = new GameObject("Dash Direction Marker");
			DashMarker.transform.parent = Rig.transform;
			DashMarker.transform.position = Rig.transform.position;
			DashMarker.transform.rotation = Rig.transform.rotation;
		}

		if (SoundSource != null && IdleSound != null) {
			SoundSource.clip = IdleSound;
			SoundSource.Play();
			Debug.Log ("1");
		}
	}
	
	
	
	void FixedUpdate () {

		if (DrainFuel) {
			FuelSupply -= MoveCost;
			if (FuelSupply < 0) {
				FuelDepleted = true;
				FuelSupply = 0;
			}
		}

		if (RechargeFuel) {
			FuelSupply += FuelRecharge;
			if (FuelSupply >= FuelNeeded) {
				FuelDepleted = false;
			}
		}
	

		if (DisableForwardMove == false) {
			Rig.AddForce (FacingDirection * Throttle, ForceMode.Acceleration);

		}

		if (DisableSideMove == false) {
			if (!SideMoveRotates) {
				Rig.AddForce (SideDirection * Throttle, ForceMode.Acceleration);
			}
			if (SideMoveRotates) {
				Rig.transform.localRotation = Rig.transform.localRotation * Quaternion.Euler (0, SideMove * RotationSpeed, 0);
			}


		}
		if (DisableVerticalMove == false) {
			Rig.AddForce (VerticalDirection * Throttle, ForceMode.Acceleration);
		}


		if (!Input.GetButton ("VerticalMove") && (DisableVerticalMove == false)) {
			Rig.AddForce (-Vector3.up * FallSpeed, ForceMode.Acceleration);
		}

		if (DashBool && DashReady) {
			DashDirection = DashMarker.transform.position - Rig.transform.position;
			Rig.velocity = Vector3.zero;
			Rig.AddForce(DashDirection * DashSpeed, ForceMode.Acceleration);
			DashBool = false;
			DashReady = false;
		}

	}
	
	
	
	void Update(){



		////Basic Movement

			if (DisableSideMove == false) {
				SideMove = (Input.GetAxis ("SideMove"));
			} else {
				SideMove = 0;
			}

			if (DisableForwardMove == false) {
				ForwardMove = (Input.GetAxis ("ForwardMove"));
			} else {
				ForwardMove = 0;
			}

			if (DisableVerticalMove == false) {
				VerticalMove = (Input.GetAxis ("VerticalMove"));
			} else {
				VerticalMove = 0;
			}
		


		if (UseFuel) {
			if (!FuelCostOnVerticalMove) {
				if ((Input.GetAxisRaw ("ForwardMove") != 0 || Input.GetAxis ("SideMove") != 0 || Input.GetAxis ("VerticalMove") != 0) && (FuelDepleted == false)) {
					RechargeFuel = false;
					DrainFuel = true;
				}
			}

			if (FuelCostOnVerticalMove) {
				if ((Input.GetAxis ("VerticalMove") != 0) && (FuelDepleted == false)) {
					DrainFuel = true;
					RechargeFuel = false;
				}
			}


		


			if (FuelCostOnVerticalMove) {
				if (Input.GetAxisRaw ("VerticalMove") == 0 || FuelDepleted) {
					DrainFuel = false;
					RechargeFuel = true;
				}

			}
			if (!FuelCostOnVerticalMove) {
				if ((Input.GetAxisRaw ("VerticalMove") == 0 && Input.GetAxisRaw ("SideMove") == 0 && Input.GetAxisRaw ("ForwardMove") == 0) || FuelDepleted) {
					DrainFuel = false;
					RechargeFuel = true;
				}
			
			}
		
		}



		if (Throttle != tempThrottle) {
			fullSpeed = Throttle;
			halfSpeed = Throttle * 0.707106682f;     	
		}

		if (SpotShadow != null) {
			if(Physics.Raycast(Rig.transform.position, -Rig.transform.up, out Spothit, 100000, SpotShadowSurfaces)){
				Spot.SetActive(true);
				Spotpoint = new Vector3(Spothit.point.x, Spothit.point.y + 0.05f, Spothit.point.z);
				Spot.transform.position = Spotpoint;
				Spot.transform.eulerAngles = Vector3.zero;
			}
			else
			{
				Spot.SetActive(false);
			}
		}
		
		
		
		//first of all - if something is pressed, suppress the Drag mechanic, so it won't slow us down.
		if (SideMove  == 0 && VerticalMove == 0 && ForwardMove == 0) {
			Rig.drag = StaticDrag;
		} else {
			Rig.drag = DynamicDrag;
		}
		
		
		
		//Throttle speed regulation for multiple input and set DashMarker.
		
		if(Input.GetButtonDown("SideMove") && DisableSideMove ==false){
			InputCount += 1;

		}
		if(Input.GetButtonUp("SideMove") && DisableSideMove ==false){
			InputCount -= 1;
		}

		
		
		if(Input.GetButtonDown("ForwardMove") && DisableForwardMove ==false){
			InputCount += 1;
		}
		if(Input.GetButtonUp("ForwardMove") && DisableForwardMove ==false){
			InputCount -= 1;
		}


		
		
		if(InputCount == 0 || InputCount == 1){
			Throttle = fullSpeed;
		}
		if(InputCount == 2){
			Throttle = halfSpeed;
		}

		if ((InputCount > 0 || Input.GetAxisRaw("VerticalMove") == 1 || Input.GetAxisRaw("VerticalMove") == -1) && (!UseFuel || !FuelDepleted) && SoundSource != null && ThrottleSound != null && SoundSource.isPlaying == false && IdleSound == null) {
		
				SoundSource.clip = ThrottleSound;
				SoundSource.Play();
		}
		if ((InputCount > 0 || Input.GetAxisRaw("VerticalMove") == 1 || Input.GetAxisRaw("VerticalMove") == -1) && (!UseFuel || !FuelDepleted) && SoundSource != null && ThrottleSound != null  && IdleSound != null && SoundSource.clip != ThrottleSound) {
			
			SoundSource.clip = ThrottleSound;
			SoundSource.Play();
		}


		if (InputCount == 0 && Input.GetAxisRaw("VerticalMove") == 0 && SoundSource != null && IdleSound != null && SoundSource.clip != IdleSound) {
			SoundSource.clip = IdleSound;
			SoundSource.Play();
		}
		if (InputCount == 0 && Input.GetAxisRaw("VerticalMove") == 0 && SoundSource != null && IdleSound == null && SoundSource.isPlaying == true) {
			SoundSource.Stop ();
		}

		
		
		if ((!UseFuel) || FuelDepleted == false) {
			FacingDirection = Rig.transform.forward * ForwardMove;
			SideDirection = Rig.transform.right * SideMove;
			VerticalDirection = Vector3.up * VerticalMove; //it's not local! Leave it!
		}
		if (UseFuel && FuelDepleted && !FuelCostOnVerticalMove) {
			FacingDirection = Vector3.zero ;
			SideDirection = Vector3.zero;
			VerticalDirection = Vector3.zero ; //it's not local! Leave it!
		}
		if (UseFuel && FuelDepleted && FuelCostOnVerticalMove) {
			FacingDirection = Rig.transform.forward * ForwardMove;
			SideDirection = Rig.transform.right * SideMove;
			VerticalDirection = Vector3.zero ; //it's not local! Leave it!
		}

		

		
		
		if(Rig.velocity.magnitude > MaxSpeed)
		{
			Rig.velocity = Rig.velocity.normalized * MaxSpeed;
		}
		






		/////
		/// Dash Marker position - it indicates the direction of Dash jump.
		/// 


		if(SideMove == 0 && DashMarker != null) 
		{
			DashMarker.transform.localPosition = new Vector3(0,DashMarker.transform.localPosition.y,DashMarker.transform.localPosition.z);  
		}
		if(SideMove == 1 && DashMarker != null) 
		{
			DashMarker.transform.localPosition = new Vector3(5,DashMarker.transform.localPosition.y,DashMarker.transform.localPosition.z);  
		}
		if(SideMove == -1 && DashMarker != null)  
		{
			DashMarker.transform.localPosition = new Vector3(-5,DashMarker.transform.localPosition.y,DashMarker.transform.localPosition.z);  
		}
		
		
		
		if(ForwardMove == 0 && DashMarker != null) 
		{
			DashMarker.transform.localPosition = new Vector3(DashMarker.transform.localPosition.x,DashMarker.transform.localPosition.y,0);
		}
		if(ForwardMove == 1 && DashMarker != null) 
		{
			DashMarker.transform.localPosition = new Vector3(DashMarker.transform.localPosition.x,DashMarker.transform.localPosition.y,5);
		}
		if(ForwardMove == -1 && DashMarker != null) 
		{
			DashMarker.transform.localPosition = new Vector3(DashMarker.transform.localPosition.x,DashMarker.transform.localPosition.y,-5);
		}


		if (AllowVerticalDash) {
			if (VerticalMove == 0 && DashMarker != null) {
				DashMarker.transform.localPosition = new Vector3 (DashMarker.transform.localPosition.x, 0, DashMarker.transform.localPosition.z);
			}
			if (VerticalMove == 1 && DashMarker != null) {
				DashMarker.transform.localPosition = new Vector3 (DashMarker.transform.localPosition.x, 5, DashMarker.transform.localPosition.z);
			}
			if (VerticalMove == -1 && DashMarker != null) {
				DashMarker.transform.localPosition = new Vector3 (DashMarker.transform.localPosition.x, -5, DashMarker.transform.localPosition.z);
			}
		}


		if (Input.GetButtonDown ("Dash") && DashMarker != null && DashReady == true && DashMarker.transform.position != Rig.transform.position) 
		{
			if ((UseFuel && FuelSupply >= DashCost) || (!UseFuel)) {
				if (AllowVerticalDash == true) {
					if (InputCount > 0 || Input.GetAxis ("VerticalMove") != 0) {
						DashBool = true;
						StartCoroutine ("DashCooldownFunction");
						SoundSource.PlayOneShot(DashSound);
					}
				}
			

				if (AllowVerticalDash == false) {
					if (Input.GetAxis ("VerticalMove") == 0) {
						DashBool = true;
						StartCoroutine ("DashCooldownFunction");
						SoundSource.PlayOneShot(DashSound);
					}
				}

				if (UseFuel) {
					FuelSupply -= DashCost;
					if (FuelSupply < 0) {
						FuelSupply = 0;
					}
				}
			}
		}



		tempThrottle = Throttle;

		if (FuelSupply > MaxFuelSupply) {
			FuelSupply = MaxFuelSupply;
		}
	}


	IEnumerator DashCooldownFunction()
	{
		yield return new WaitForSeconds(DashInterval);
		DashReady = true;
	}

}