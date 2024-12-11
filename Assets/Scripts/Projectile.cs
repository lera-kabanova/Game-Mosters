using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    /// <summary>
    /// The projectile's target
    /// </summary>
    private Monster target;

    /// <summary>
    /// The projectile's tower
    /// </summary>
    private Tower parent;

    /// <summary>
    /// The projectile's animator
    /// </summary>
    private Animator myAnimator;


    /// <summary>
    /// The element type of the projectile
    /// </summary>
    private Element elementType;

    // Use this for initialization
    void Start()
    {
        //Creates a reference to the animator
        myAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveToTarget();
    }

    public void Initialize(Tower parent)
    {
        this.target = parent.Target;
        this.parent = parent;
        this.elementType = parent.ElementType;
    }

    private void MoveToTarget()
    {
        if (target != null && target.IsActive) //If the target isn't null and the target isn't dead
        {
            //Move towards the position
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, Time.deltaTime * parent.ProjectileSpeed);

            //Calculates the direction of the projectile
            Vector2 dir = target.transform.position - transform.position;

            //Calculates the angle of the projectile
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            //Sets the rotation based on the angle
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        else if (!target.IsActive)
        {
            GameManager.Instance.Pool.ReleaseObject(gameObject);
        }
    }

    /// <summary>
    /// Tries to apply a debuff to the target
    /// </summary>
    private void ApplyDebuff()
    {
        //Checks if the target is immune to the debuff
        if (target.ElementType != elementType)
        {
            //Does a roll to check if we have to apply a debuff
            float roll = Random.Range(0, 100);

            if (roll <= parent.Proc)
            {
                //applies the debuff
                target.AddDebuff(parent.GetDebuff());
            }
        }


    }

    /// <summary>
    /// When the projectile hits something
    /// </summary>
    /// <param name="other">The object the projectil hit</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        //If we hit a monster
        if (other.tag == "Monster")
        {
            if (target.gameObject == other.gameObject)
            {
                //Makes the monster take damage based on the tower stats
                target.TakeDamage(parent.Damage, elementType);

                //Triggers the impact animation
                myAnimator.SetTrigger("Impact");

                //Tries to apply a debuff
                ApplyDebuff();
            }


        }

    }


}
