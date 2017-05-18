using UnityEngine;
using System.Collections;

public class uJet_Fuel_Pickup : MonoBehaviour {

	[Tooltip("INFO: for this script to work, add a Collider set as Trigger to this GameObject. It then becomes a Fuel Recharge Pickup! Just remember to set it's Layer to one which collides only with the Player - unless you want the bullets or enemies to trigger this pickup! [Additional Info:] To minimalise Triggers not being triggered at very high speeds, you can try adding more than one Trigger Collider of the same type, just with different size, to create more than one layer of Trigger Collider.   [PS:] This is just a Tooltip variable. It changes nothing - it's just for the instructions")]
	public bool Info;
	[Space(15)]
	[Tooltip("Quite self explaining. How many seconds need to pass for this pickup to re-appear in the world after it was picked up?")]
	public float RespawnTime;
	[Tooltip("How much fuel do you want it to recharge?")]
	public float FuelGiven;
	[Tooltip("If you want, you can set a Mesh Renderer to disappear, when the PickUp is, well, picked up.")]
	public MeshRenderer MeshToDisappear;
	[Tooltip("If you want, you can set a Collider to disappear, when the PickUp is picked up.")]
	public Collider TriggerToDisappear;
	[Tooltip("You can also set a whole GameObject (like a child which holds particles and a mesh model of the pickup) to disappear, when the PickUp is picked up.")]
	public GameObject ObjectToDisappear;
	[Tooltip("You can set a sound to be played upon picking this up. Just attach the AudioSource and select the sound clip!")]
	public AudioSource SoundSource;
	[Tooltip("The sound clip of PickUp being, well, picked up.")]
	public AudioClip PickUpSound;

	private float t;
	private bool pickedup;
	private Collider d;

	// Update is called once per frame
	void OnTriggerEnter ( Collider c) 
	{
		if (c != d) 
		{
			if (SoundSource != null) {
				SoundSource.PlayOneShot(PickUpSound);
			}
			if (MeshToDisappear != null) {
				MeshToDisappear.enabled = false;
			}
			if (TriggerToDisappear != null) {
				TriggerToDisappear.enabled = false;
			}
			if (ObjectToDisappear != null) {
				ObjectToDisappear.SetActive (false);
			}
			uJet_Jetpack_Controller Jet;
			Jet = c.gameObject.GetComponent<uJet_Jetpack_Controller> ();
			Jet.FuelSupply += FuelGiven;
			if (Jet.FuelSupply > Jet.MaxFuelSupply) {
				Jet.FuelSupply = Jet.MaxFuelSupply;
			}
			pickedup = true;
			d = c;
		}

	}


	void Update()
	{
		if (pickedup) {
			t += Time.deltaTime;
			if(t>= RespawnTime)
			{
				d = null;
				t = 0;
				pickedup = false;
				if (MeshToDisappear != null) {
					MeshToDisappear.enabled = true;
				}
				if (TriggerToDisappear != null) {
					TriggerToDisappear.enabled = true;
				}
				if (ObjectToDisappear != null) {
					ObjectToDisappear.SetActive(true);
				}
			}
		}
	}
}
