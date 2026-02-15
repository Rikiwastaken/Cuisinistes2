using UnityEngine;

// Animation.Play example. Let the S and J keys start
// a spin or jump animation. Let Space play back spin and
// jump at the same time. Let Z play spin and jump with
// spin doubled in speed.
//
// Spin: rotate the cube 360 degrees in half or one second
// Jump: bounce up to 2 units and down in one second
//
// Note: AnimationState.layer is no longer supported, but still exists.

public class ExampleScript : MonoBehaviour
{
    private Animation anim;

    void Start()
    {
        anim = gameObject.GetComponent<Animation>();
    }

    void Update()
    {
        // leave spin or jump to complete before changing
        if (anim.isPlaying)
        {
            return;
        }

        // combine jump and spin
        Debug.Log("Jumping and spinning");
        anim.Play("animation");

    }
}