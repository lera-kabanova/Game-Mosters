using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Element { STORM, FIRE, FROST, POISON, NONE }

public abstract class Tower : MonoBehaviour
{

    [SerializeField]
    private string projectileType;

    [SerializeField]
    private float projectileSpeed;

    private Animator myAnimator;

    [SerializeField]
    private int damage;

    [SerializeField]
    private float debuffDuration;

    [SerializeField]
    private float proc;

    private SpriteRenderer mySpriteRenderer;

    private Monster target;

    private Queue<Monster> monsters = new Queue<Monster>();

 
    private bool canAttack = true;

    private float attackTimer;

    [SerializeField]
    private float attackCooldown;

    public int Level { get; protected set; }

    public TowerUpgrade[] Upgrades { get; protected set; }

    public Element ElementType { get; protected set; }

    public int Price { get; set; }

    public float ProjectileSpeed
    {
        get { return projectileSpeed; }
    }

    public Monster Target
    {
        get { return target; }
    }

    public int Damage
    {
        get
        {
            return damage;
        }
    }

    public float DebuffDuration
    {
        get
        {
            return debuffDuration;
        }

        set
        {
            this.debuffDuration = value;
        }
    }

    public float Proc
    {
        get
        {
            return proc;
        }

        set
        {
            this.proc = value;
        }
    }

    public TowerUpgrade NextUpgrade
    {
        get
        {
            if(Upgrades.Length > Level - 1)
            {
                return Upgrades[Level - 1];
            }
            return null;
        }
    }

    // Use this for initialization
    void Awake()
    {
        myAnimator = transform.parent.GetComponent<Animator>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        Level = 1;
    }

    // Update is called once per frame
    void Update()
    {
        Attack();
    }

    public void Select()
    {
        mySpriteRenderer.enabled = !mySpriteRenderer.enabled;
        GameManager.Instance.UpdateUpgradeTip();
    }

    private void Attack()
    {
        if (!canAttack)//If we can't attack
        {
            //Count how much time has passed since last attack
            attackTimer += Time.deltaTime;

            //If the time passed is higher than the cooldown, then we need to reset
            //and be able to attack again
            if (attackTimer >= attackCooldown)
            {
                canAttack = true;
                attackTimer = 0;
            }
        }
        //If we don't have a target and we have more targets in range
        if (target == null && monsters.Count > 0 && monsters.Peek().IsActive) 
        {
            target = monsters.Dequeue();

        }
        if (target != null && target.IsActive)//If we have a target that is active
        {
            if (canAttack)//If we can attack then we shoot at the target
            {
                Shoot();

                myAnimator.SetTrigger("Attack");

                canAttack = false;
            }

        }
      
        if (target != null && !target.Alive || target != null && !target.IsActive)
        {
            target = null;
        }

    }

    public virtual string GetStats()
    {
        if(NextUpgrade != null)
        {
            return string.Format("\nLevel: {0} \nDamage: {1} <color=#00ff00ff> +{4}</color>\nProc: {2}% <color=#00ff00ff>+{5}%</color>\nDebuff: {3}sec <color=#00ff00ff>+{6}</color>", Level, damage, proc, DebuffDuration, NextUpgrade.Damage, NextUpgrade.ProcChance, NextUpgrade.DebuffDuration);
        }
        return string.Format("\n Level: {0} \nDamage: {1} \nProc: {2}% \nDebuff: {3}sec", Level, damage, proc, DebuffDuration);
    }
    private void Shoot()
    {
        //Gets a projectile from the object pool
        Projectile projectile = GameManager.Instance.Pool.GetObject(projectileType).GetComponent<Projectile>();

        //Makes sure that the projectile is instantiated on the towers position
        projectile.transform.position = transform.position;

        projectile.Initialize(this);
    }

    public virtual void Upgrade()
    {
        GameManager.Instance.Currency -= NextUpgrade.Price;
        Price += NextUpgrade.Price;
        this.damage += NextUpgrade.Damage;
        this.proc += NextUpgrade.ProcChance;
        this.DebuffDuration += NextUpgrade.DebuffDuration;
        Level++;
        GameManager.Instance.UpdateUpgradeTip();
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Monster") //Adds new monsters to the queu when they enter the range
        {
            monsters.Enqueue(other.GetComponent<Monster>());
        }
    }

    public abstract Debuff GetDebuff();

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Monster")
        {
            target = null;
        }
    }
}
    